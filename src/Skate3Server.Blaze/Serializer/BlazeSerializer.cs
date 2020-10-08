﻿using System;
using System.Buffers;
using System.IO;
using System.Text;
using NLog;

namespace Skate3Server.Blaze.Serializer
{
    public interface IBlazeSerializer
    {
        object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType);
        void Serialize(Stream output, BlazeHeader header, object payload);
    }

    public class BlazeSerializer : IBlazeSerializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BlazeSerializer()
        {
        }

        public object Deserialize(ref ReadOnlySequence<byte> payload, Type requestType)
        {
            var request = Activator.CreateInstance(requestType);

            var payloadReader = new SequenceReader<byte>(payload);
            var payloadStringBuilder = new StringBuilder();

            var inStruct = false;
            while (!payloadReader.End)
            {
                var label = TdfHelper.ParseLabel(ref payloadReader);
                var typeData = TdfHelper.ParseTypeAndLength(ref payloadReader);
                var type = typeData.Item1;
                var length = typeData.Item2;

                payloadStringBuilder.AppendLine($"{label} {type} {length}");

                switch (type)
                {
                    case TdfType.Struct:
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<start struct>");
                        inStruct = true;
                        break;
                    case TdfType.String:
                        var byteStr = payloadReader.Sequence.Slice(payloadReader.Position, length);
                        payloadReader.Advance(length);
                        //TODO: figure out if utf8 is correct
                        var str = Encoding.UTF8.GetString(byteStr.ToArray());
                        payloadStringBuilder.AppendLine($"{str}");
                        break;
                    case TdfType.Int8:
                        payloadReader.TryRead(out byte int8);
                        payloadStringBuilder.AppendLine($"{int8}");
                        break;
                    case TdfType.Uint8:
                        payloadReader.TryRead(out byte uint8);
                        payloadStringBuilder.AppendLine($"{uint8}");
                        break;
                    case TdfType.Int16:
                        payloadReader.TryReadBigEndian(out short int16);
                        payloadStringBuilder.AppendLine($"{int16}");
                        break;
                    case TdfType.Uint16:
                        payloadReader.TryReadBigEndian(out short uint16);
                        payloadStringBuilder.AppendLine($"{Convert.ToUInt16(uint16)}");
                        break;
                    case TdfType.Int32:
                        payloadReader.TryReadBigEndian(out int int32);
                        payloadStringBuilder.AppendLine($"{int32}");
                        break;
                    case TdfType.Uint32:
                        payloadReader.TryReadBigEndian(out int uint32);
                        payloadStringBuilder.AppendLine($"{Convert.ToUInt32(uint32)}");
                        break;
                    case TdfType.Int64:
                        payloadReader.TryReadBigEndian(out long int64);
                        payloadStringBuilder.AppendLine($"{int64}");
                        break;
                    case TdfType.Uint64:
                        payloadReader.TryReadBigEndian(out long uint64);
                        payloadStringBuilder.AppendLine($"{Convert.ToUInt64(uint64)}");
                        break;
                    case TdfType.Array:
                        //TODO
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<array>");
                        break;
                    case TdfType.Blob:
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<blob>");
                        break;
                    case TdfType.Map:
                        payloadReader.Advance(length);
                        payloadStringBuilder.AppendLine($"<map>");
                        break;
                    case TdfType.Union:
                        payloadReader.Advance(length);
                        payloadReader.TryRead(out byte key);
                        //TODO: handle VALU better
                        payloadStringBuilder.AppendLine($"<union>");
                        break;
                    default:
                        Logger.Debug($"Partial Decode:{Environment.NewLine}{payloadStringBuilder}");
                        throw new ArgumentOutOfRangeException();
                }

                //end of struct detection (not great and may break)
                if (inStruct && payloadReader.TryPeek(out var nextByte) && nextByte == 0x0)
                {
                    payloadStringBuilder.AppendLine($"<end struct>");
                    payloadReader.Advance(1);
                    inStruct = false;
                }
            }

            Logger.Debug($"Decoded:{Environment.NewLine}{payloadStringBuilder}");
            return request;
        }

        public void Serialize(Stream output, BlazeHeader header, object payload)
        {
            //throw new NotImplementedException();
        }
    }
}
