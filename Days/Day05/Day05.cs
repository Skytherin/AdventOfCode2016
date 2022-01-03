using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day05
{
    [UsedImplicitly]
    public class Day05: IAdventOfCode
    {
        public void Run()
        {
            Part1();
            Part2();
        }

        private void Part1()
        {
            GetPassword("abc").ToLower().Should().Be("18f47a30");
            GetPassword("ojvtpuvg").ToLower().Should().Be("4543c154");
        }

        private void Part2()
        {
            GetPassword2("abc").ToLower().Should().Be("05ace8e3");
            GetPassword2("ojvtpuvg").ToLower().Should().Be("1050cbbd");
        }

        private string GetPassword(string doorId)
        {
            var pw = "";
            for (var i = 0; pw.Length < 8; i++)
            {
                var hash = BitConverter.ToString(MD5.HashData(Encoding.ASCII.GetBytes($"{doorId}{i}"))).Replace("-", "");
                if (hash.StartsWith("00000"))
                {
                    pw += hash[5];
                }
            }

            return pw;
        }

        private string GetPassword2(string doorId)
        {
            var pw = new Dictionary<int, char>();
            for (var i = 0; pw.Count < 8; i++)
            {
                var hash = BitConverter.ToString(MD5.HashData(Encoding.ASCII.GetBytes($"{doorId}{i}"))).Replace("-", "");
                if (hash.StartsWith("00000"))
                {
                    if (!new[] {'0', '1', '2', '3', '4', '5', '6', '7' }.Contains(hash[5])) continue;
                    var position = Convert.ToInt32($"{hash[5]}");
                    var value = hash[6];
                    if (!pw.ContainsKey(position))
                    {
                        pw[position] = value;
                    }
                }
            }

            return pw.OrderBy(it => it.Key).Select(it => it.Value).Join();
        }
    }
}