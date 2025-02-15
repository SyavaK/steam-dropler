﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using steam_dropler.Steam;

namespace steam_dropler.Model
{
    public class AccountConfig
    {
        /// <summary>
        /// Имя аккаунта
        /// </summary>
        [JsonIgnore]
        public string Name { get; set; }

        /// <summary>
        /// Пароль аккаунта
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Steam id
        /// </summary>
        public ulong? SteamId { get; set; }

        /// <summary>
        /// Участвует в фарме
        /// </summary>
        public bool IdleEnable { get; set; }

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime? LastRun { get; set; }

        /// <summary>
        /// Идлит ли сейчас бот
        /// </summary>
        public bool IdleNow { get; set; }

		/// <summary>
		/// Мобильный аутентификатор
		/// </summary>
		[JsonIgnore]
		public MobileAuth MobileAuth { get; set; }

        /// <summary>
        /// Путь до файла настроек 
        /// </summary>
        private string FilePath { get; }

        /// <summary>
        /// Ключ для авторизации
        /// </summary>
        public string LoginKey { get; set; }

        /// <summary>
        /// Hash для входа
        /// </summary>
        public byte[] SentryHash { get; set; }

        /// <summary>
        /// Упрощенный вход sda
        /// </summary>
        public string SharedSecret { get; set; }

        /// <summary>
        /// Правила дропа
        /// </summary>
        public List<(uint, ulong)> DropConfig { get; set; }

        [JsonIgnore]
        public List<uint> AppIds => DropConfig?.Select(t=>t.Item1).ToList();

        

        /// <summary>
        /// Конструктор для json
        /// </summary>
        public AccountConfig()
        {
        }

        /// <summary>
        /// Конструктор по файлу
        /// </summary>
        /// <param name="path"></param>
        public AccountConfig(string path)
        {
           var obj = JsonConvert.DeserializeObject<AccountConfig>(File.ReadAllText(path));

            Password = obj.Password;
            SteamId = obj.SteamId;
            IdleEnable = obj.IdleEnable;
            DropConfig = obj.DropConfig ?? new List<(uint, ulong)>();
            SentryHash = obj.SentryHash;
            LoginKey = obj.LoginKey;
            IdleNow = obj.IdleNow;
            LastRun = obj.LastRun ?? DateTime.MinValue;
            SharedSecret = obj.SharedSecret;
            if (SharedSecret != null)
            {
                MobileAuth = new MobileAuth {SharedSecret = obj.SharedSecret};
            }

            if (IdleNow )
            {
                IdleNow = false;
                if ((DateTime.UtcNow - LastRun.Value).TotalHours < 10)
                {
                    LastRun = DateTime.MinValue;
                }
            }


            Name = Path.GetFileNameWithoutExtension(path);
            FilePath = path;
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }

    }
}
