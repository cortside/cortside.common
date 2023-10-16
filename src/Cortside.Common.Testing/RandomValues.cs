// TODO: ability to populate lists with loaded values, i.e. json file??  could even be just set them
// TODO: ability to get instance that differs from static??

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cortside.Common.Testing {
    public static class RandomValues {
        private static readonly Random random = new Random();
        private static readonly List<string> firstNameList = new List<string> { "Linda", "Bob", "Peter", "Michelle", "Zack", "James", "John", "Robert", "William", "David", "Joseph", "Thomas", "Charles", "Michael", "Emma", "Olivia", "Isabella", "Sophia", "Hannah", "Mary", "Jane", "Emily", "Victoria" };
        private static readonly List<string> lastNameList = new List<string> { "Anderson", "Smith", "Richards", "Howell", "Fleming", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson" };
        private static readonly List<string> cityList = new List<string> { "Maryland", "New York", "Miami", "Salt Lake City", "Portland", "Seattle", "Las Vegas", "San Francisco", "Austin", "Boston", "Los Angeles", "Denver", "San Diego", "Minneapolis", "Kansas City", "Orlando" };
        private static readonly List<string> stateList = new List<string> { "TX", "AK", "ID", "VA", "TN", "FL", "AL", "GA", "CO", "NM", "CA", "NY", "MT", "KY", "AZ", "RI" };
        private static readonly List<string> streetList = new List<string> { "Elm", "Main", "State", "Constitution", "Washington", "Park", "Lake", "Hill" };
        private static readonly List<string> streetSuffixList = new List<string> { "ST", "BLVD", "LN", "CT", "DR", "HWY", "LOOP", "WAY" };

        public static string CreateRandomNumberString(int length = 10) {
            const string chars = "1234567890";

            string str = new string(Enumerable.Repeat(chars, length)
               .Select(s => s[random.Next(s.Length)]).ToArray());

            return str;
        }

        public static string CreateRandomString(int lengthStart = 10, int lengthEnd = 20, bool useSpecialChars = false) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

            string str = new string(Enumerable.Repeat(chars, random.Next(lengthStart, lengthEnd))
               .Select(s => s[random.Next(s.Length)]).ToArray());

            if (!useSpecialChars || str.Length == 0) {
                return str;
            }

            int whichSet = random.Next(0, 4);
            char specialChar = whichSet switch {
                0 => (char)(32 + random.Next(0, 16)),
                1 => (char)(58 + random.Next(0, 7)),
                2 => (char)(91 + random.Next(0, 6)),
                3 => (char)(123 + random.Next(0, 4)),
                _ => (char)(32 + random.Next(0, 16))
            };
            return str[1..] + specialChar;
        }

        public static string FullName {
            get {
                return $"{FirstName} {LastName}";
            }
        }

        public static string FirstName {
            get {
                return firstNameList[random.Next(0, firstNameList.Count)];
            }
        }

        public static string LastName {
            get {
                return lastNameList[random.Next(0, lastNameList.Count)];
            }
        }

        public static string City {
            get {
                return cityList[random.Next(0, cityList.Count)];
            }
        }

        public static string State {
            get {
                return stateList[random.Next(0, stateList.Count)];
            }
        }

        public static string ZipCode {
            get {
                return Number(11111, 99999).ToString();
            }
        }

        public static string AddressLine1 {
            get {
                return $"{random.Next(1, 9999)} {streetList[random.Next(0, streetList.Count)]} {streetSuffixList[random.Next(0, streetSuffixList.Count)]}";
            }
        }

        public static string EmailAddress {
            get {
                return $"{CreateRandomString()}@{CreateRandomString()}.{CreateRandomString(3, 3)}";
            }
        }

        public static string PhoneNumber() {
            return GetLong(2000000000, 2147483647).ToString();
        }

        public static string SocialSecurityNumber() {
            return GetLong(100000000, 899999999).ToString();
        }

        public static DateTime Date(int yearStart = 1940, int yearEnd = 2000) {
            return new DateTime(Number(yearStart, yearEnd), Number(1, 13), Number(1, 29), 0, 0, 0, DateTimeKind.Local);
        }

        public static int Number(int start = 0, int end = 9999, int multiplier = 1) {
            return random.Next(start, end) * multiplier;
        }

        public static long GetLong(long minValue = 1111111111, long maxValue = 9999999999) {
            int shift = 0;
            long test = maxValue;
            while (test != (int)test) {
                shift++;
                test >>= 1;
            }

            long result = random.Next((int)(minValue >> shift), (int)(maxValue >> shift));
            result <<= shift;
            return result;
        }
    }
}
