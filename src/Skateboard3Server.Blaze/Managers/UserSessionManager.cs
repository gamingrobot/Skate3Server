﻿using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading;
using NLog;
using Skateboard3Server.Data.Models;

namespace Skateboard3Server.Blaze.Managers
{
    public interface IUserSessionManager
    {
        (uint Id, string SessionKey) CreateSession(uint userId);
        void RemoveSession(uint id);
        UserSessionData GetUserSessionDataForKey(string key);
        string GetSessionKey(uint id);
    }

    /// <summary>
    /// Current sessions connected to the server
    /// the sessions die with the server so we are fine with keeping them in memory
    /// </summary>
    public class UserSessionManager : IUserSessionManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<uint, UserSessionData> _sessions =
            new ConcurrentDictionary<uint, UserSessionData>();

        private int _currentSessionCount = 1;

        private static readonly RNGCryptoServiceProvider RandomGenerator = new RNGCryptoServiceProvider();


        public (uint Id, string SessionKey) CreateSession(uint userId)
        {
            var rawKey = new byte[16];
            RandomGenerator.GetBytes(rawKey);
            var id = (uint) Interlocked.Increment(ref _currentSessionCount); //generate a session id
            var sessionData = new UserSessionData
            {
                UserId = userId,
                Key = HexString(rawKey)
            };
            _sessions.TryAdd(id, sessionData);

            var sessionKey = GenerateSessionKey(id, sessionData.Key);

            return (id, sessionKey);
        }

        public void RemoveSession(uint id)
        {
            _sessions.TryRemove(id, out _);
        }

        public UserSessionData GetUserSessionDataForKey(string key)
        {
            //split lookup id, validate byte
            var parts = key.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                Logger.Warn("Invalid session key!");
                return null;
            }

            var encodedSessionId = parts[0];
            var encodedKey = parts[1];

            var sessionId = Convert.ToUInt32(encodedSessionId, 16);

            if (_sessions.TryGetValue(sessionId, out var sessionData))
            {
                if (encodedKey.Equals(sessionData.Key))
                {
                    return sessionData;
                }
            }

            return null;
        }

        public string GetSessionKey(uint id)
        {
            if (_sessions.TryGetValue(id, out var sessionData))
            {
                return GenerateSessionKey(id, sessionData.Key);
            }

            throw new ArgumentException("Session Key does not exist");
        }

        private string GenerateSessionKey(uint id, string key)
        {
            return id.ToString("X8") + "_" + key;
        }

        private string HexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    public class UserSessionData
    {
        public uint UserId { get; set; }
        public string Key { get; set; }
    }
}
