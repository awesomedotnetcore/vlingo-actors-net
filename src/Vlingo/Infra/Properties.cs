using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Vlingo.Infra
{
    public class Properties
        : Dictionary<string, string>
    {
        public string GetProperty(string key, string defaultValue = null)
        {
            return TryGetValue(key, out var v) ? v : defaultValue;
        }

        public void Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }


            var reader = new StreamReader(stream);
            while (reader.EndOfStream == false)
            {
                var line = reader.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var split = line.Split(new[] { "=" }, StringSplitOptions.None);
                if (!split.Any())
                {
                    continue;
                }
                var key = split[0];
                var value = string.Join("=", split.Skip(1));
                Add(key, value);
            }

        }
    }
}
