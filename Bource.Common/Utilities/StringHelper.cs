using Pluralize.NET.Core;
using System;
using System.Text.RegularExpressions;

namespace Bource.Common.Utilities
{
    public static class StringHelper
    {
        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        public static int ToInt(this string value)
        {
            return Convert.ToInt32(value);
        }

        public static decimal ToDecimal(this string value)
        {
            return Convert.ToDecimal(value);
        }

        public static string ToNumeric(this int value)
        {
            return value.ToString("N0"); //"123,456"
        }

        public static string ToNumeric(this decimal value)
        {
            return value.ToString("N0");
        }

        public static string ToCurrency(this int value)
        {
            //fa-IR => current culture currency symbol => ریال
            //123456 => "123,123ریال"
            return value.ToString("C0");
        }

        public static string ToCurrency(this decimal value)
        {
            return value.ToString("C0");
        }

        /// <summary>
        /// Singularizin name like Posts to Post or People to Person
        /// </summary>
        /// <param name="name"></param>
        public static string SingularizingNameConvention(this string name)
        {
            Pluralizer pluralizer = new Pluralizer();
            return pluralizer.Singularize(name);
        }

        /// <summary>
        /// Pluralizing name like Post to Posts or Person to People
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string PluralizingNameConvention(this string name)
        {
            Pluralizer pluralizer = new Pluralizer();
            return pluralizer.Pluralize(name);
        }

        /// <summary>
        /// Replace arabic numbers with english numbers
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static string ArToEngNumbers(this string numbers)
        {
            char[] find = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
            char[] replace = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < find.Length; i++)
            {
                numbers = numbers.Replace(find[i], replace[i]);
            }

            return numbers;
        }

        /// <summary>
        /// Replace persian numbers with english number
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static string PerToEngNumbers(this string numbers)
        {
            char[] find = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
            char[] replace = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < find.Length; i++)
            {
                numbers = numbers.Replace(find[i], replace[i]);
            }

            return numbers;
        }

        /// <summary>
        /// Replace english numbers with persian number
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static string EngToPerNumbers(this string numbers)
        {
            char[] find = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] replace = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
            for (int i = 0; i < find.Length; i++)
            {
                numbers = numbers.Replace(find[i], replace[i]);
            }

            return numbers;
        }

        /// <summary>
        /// Compare two persion letter
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ComparePersion(string a, string b)
        {
            a = FixPersianLetters(a);
            b = FixPersianLetters(b);

            if (a.Replace(" ", string.Empty).Replace('آ', 'ا') == b.Replace(" ", string.Empty).Replace('آ', 'ا'))
                return true;

            return false;
        }

        /// <summary>
        /// Fixed multiform persion letters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FixPersianLetters(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            char[] find;
            char replace;
            find = new char[] {'؆', '؇', '؈', '؉', '؊', '؍', '؎', 'ؐ', 'ؑ', 'ؒ', 'ؓ', 'ؔ', 'ؕ',
            'ؖ', 'ؘ', 'ؙ', 'ؚ', '؞', 'ٖ', 'ٗ', '٘', 'ٙ', 'ٚ', 'ٛ', 'ٜ', 'ٝ', 'ٞ', 'ٟ', '٪',
            '٬', '٭', 'ہ', 'ۂ', 'ۃ', '۔', 'ۖ', 'ۗ', 'ۘ', 'ۙ', 'ۚ', 'ۛ', 'ۜ', '۞', '۟', '۠',
            'ۡ', 'ۢ', 'ۣ', 'ۤ', 'ۥ', 'ۦ', 'ۧ', 'ۨ', '۩', '۪', '۫', '۬', 'ۭ', 'ۮ', 'ۯ', 'ﮧ',
            '﮲', '﮳', '﮴', '﮵', '﮶', '﮷', '﮸', '﮹', '﮺', '﮻', '﮼', '﮽', '﮾', '﮿', '﯀', '﯁', 'ﱞ',
            'ﱟ', 'ﱠ', 'ﱡ', 'ﱢ', 'ﱣ', 'ﹰ', 'ﹱ', 'ﹲ', 'ﹳ', 'ﹴ', 'ﹶ', 'ﹷ', 'ﹸ', 'ﹹ', 'ﹺ', 'ﹻ', 'ﹼ', 'ﹽ',
            'ﹾ', 'ﹿ' };
            text = text.Trim(find);

            find = new char[] {'أ','إ','ٱ','ٲ','ٳ','ٵ','ݳ','ݴ','ﭐ','ﭑ','ﺃ','ﺄ','ﺇ','ﺈ',
            'ﺍ','ﺎ','ﴼ','ﴽ'};
            replace = 'ا';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ٮ', 'ݕ', 'ݖ', 'ﭒ', 'ﭓ', 'ﭔ', 'ﭕ', 'ﺏ', 'ﺐ', 'ﺑ', 'ﺒ' };
            replace = 'ب';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڀ', 'ݐ', 'ݔ', 'ﭖ', 'ﭗ', 'ﭘ', 'ﭙ', 'ﭚ', 'ﭛ', 'ﭜ', 'ﭝ' };
            replace = 'پ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ٹ', 'ٺ', 'ټ', 'ݓ', 'ﭞ', 'ﭟ', 'ﭠ', 'ﭡ', 'ﭢ', 'ﭣ', 'ﭤ', 'ﭥ',
            'ﭦ', 'ﭧ', 'ﭨ', 'ﭩ', 'ﺕ', 'ﺖ', 'ﺗ', 'ﺘ' };
            replace = 'ت';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ٽ', 'ٿ', 'ݑ', 'ﺙ', 'ﺚ', 'ﺛ', 'ﺜ' };
            replace = 'ث';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڃ', 'ڄ', 'ﭲ', 'ﭳ', 'ﭴ', 'ﭵ', 'ﭶ', 'ﭷ', 'ﭸ', 'ﭹ', 'ﺝ', 'ﺞ', 'ﺟ',
            'ﺠ' };
            replace = 'ج';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڇ', 'ڿ', 'ݘ', 'ﭺ', 'ﭻ', 'ﭼ', 'ﭽ', 'ﭾ', 'ﭿ', 'ﮀ', 'ﮁ' };
            replace = 'چ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ځ', 'ݮ', 'ݯ', 'ݲ', 'ݼ', 'ﺡ', 'ﺢ', 'ﺣ', 'ﺤ' };
            replace = 'ح';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڂ', 'څ', 'ݗ', 'ﺥ', 'ﺦ', 'ﺧ', 'ﺨ' };
            replace = 'خ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڈ', 'ډ', 'ڊ', 'ڌ', 'ڍ', 'ڎ', 'ڏ', 'ڐ', 'ݙ', 'ݚ', 'ﺩ', 'ﺪ', 'ﮂ',
                'ﮃ', 'ﮈ', 'ﮉ' };
            replace = 'د';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ﱛ', 'ﺫ', 'ﺬ', 'ﮄ', 'ﮅ', 'ﮆ', 'ﮇ' };
            replace = 'ذ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { '٫', 'ڑ', 'ڒ', 'ړ', 'ڔ', 'ڕ', 'ږ', 'ݛ', 'ݬ', 'ﮌ', 'ﮍ', 'ﱜ', 'ﺭ', 'ﺮ' };
            replace = 'ر';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڗ', 'ڙ', 'ݫ', 'ݱ', 'ﺯ', 'ﺰ' };
            replace = 'ز';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ﮊ', 'ﮋ', 'ژ' };
            replace = 'ژ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ښ', 'ݽ', 'ݾ', 'ﺱ', 'ﺲ', 'ﺳ', 'ﺴ' };
            replace = 'س';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڛ', 'ۺ', 'ݜ', 'ݭ', 'ݰ', 'ﺵ', 'ﺶ', 'ﺷ' };
            replace = 'ش';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڝ', 'ﺹ', 'ﺺ', 'ﺻ', 'ﺼ' };
            replace = 'ص';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڞ', 'ۻ', 'ﺽ', 'ﺾ', 'ﺿ', 'ﻀ' };
            replace = 'ض';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ﻁ', 'ﻂ', 'ﻃ', 'ﻄ' };
            replace = 'ط';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڟ', 'ﻅ', 'ﻆ', 'ﻇ', 'ﻈ' };
            replace = 'ظ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { '؏', 'ڠ', 'ﻉ', 'ﻊ', 'ﻋ', 'ﻌ' };
            replace = 'ع';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ۼ', 'ݝ', 'ݞ', 'ݟ', 'ﻍ', 'ﻎ', 'ﻏ', 'ﻐ' };
            replace = 'غ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] {'؋', 'ڡ', 'ڢ', 'ڣ', 'ڤ', 'ڥ', 'ڦ', 'ݠ', 'ݡ', 'ﭪ', 'ﭫ', 'ﭬ', 'ﭭ',
                'ﭮ', 'ﭯ', 'ﭰ', 'ﭱ', 'ﻑ', 'ﻒ', 'ﻓ', 'ﻔ' };
            replace = 'ف';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ٯ', 'ڧ', 'ڨ', 'ﻕ', 'ﻖ', 'ﻗ', 'ﻘ', '؈' };
            replace = 'ق';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ػ', 'ؼ', 'ك', 'ڪ', 'ګ', 'ڬ', 'ڭ', 'ڮ', 'ݢ', 'ݣ', 'ݤ', 'ݿ', 'ﮎ',
                'ﮏ', 'ﮐ', 'ﮑ', 'ﯓ', 'ﯔ', 'ﯕ', 'ﯖ', 'ﻙ', 'ﻚ', 'ﻛ', 'ﻜ' };
            replace = 'ک';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڰ', 'ڱ', 'ڲ', 'ڳ', 'ڴ', 'ﮒ', 'ﮓ', 'ﮔ', 'ﮕ', 'ﮖ', 'ﮗ', 'ﮘ', 'ﮙ', 'ﮚ',
                'ﮛ', 'ﮜ', 'ﮝ'};
            replace = 'گ';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڵ', 'ڶ', 'ڷ', 'ڸ', 'ݪ', 'ﻝ', 'ﻞ', 'ﻟ', 'ﻠ' };
            replace = 'ل';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { '۾', 'ݥ', 'ݦ', 'ﻡ', 'ﻢ', 'ﻣ', 'ﻤ' };
            replace = 'م';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ڹ', 'ں', 'ڻ', 'ڼ', 'ڽ', 'ݧ', 'ݨ', 'ݩ', 'ﮞ', 'ﮟ', 'ﮠ', 'ﮡ', 'ﻥ', 'ﻦ',
                'ﻧ', 'ﻨ' };
            replace = 'ن';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ؤ', 'ٶ', 'ٷ', 'ۄ', 'ۅ', 'ۆ', 'ۇ', 'ۈ', 'ۉ', 'ۊ', 'ۋ', 'ۏ', 'ݸ', 'ݹ',
                'ﯗ', 'ﯘ', 'ﯙ', 'ﯚ', 'ﯛ', 'ﯜ', 'ﯝ', 'ﯞ', 'ﯟ', 'ﯠ', 'ﯡ', 'ﯢ', 'ﯣ', 'ﺅ', 'ﺆ', 'ﻭ', 'ﻮ' };
            replace = 'و';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ة', 'ھ', 'ۀ', 'ە', 'ۿ', 'ﮤ', 'ﮥ', 'ﮦ', 'ﮩ', 'ﮨ', 'ﮪ', 'ﮫ', 'ﮬ', 'ﮭ',
                'ﺓ', 'ﺔ', 'ﻩ', 'ﻪ', 'ﻫ', 'ﻬ' };
            replace = 'ه';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ؠ', 'ﱝ', 'ؽ', 'ؾ', 'ؿ', 'ى', 'ي', 'ٸ', 'ۍ', 'ێ', 'ې', 'ۑ', 'ے', 'ۓ',
                'ݵ', 'ݶ', 'ݷ', 'ݺ', 'ݻ', 'ﮢ', 'ﮣ', 'ﮮ', 'ﮯ', 'ﮰ', 'ﮱ', 'ﯤ', 'ﯥ', 'ﯦ', 'ﯧ', 'ﯨ',
                'ﯩ', 'ﯼ', 'ﯽ', 'ﯾ', 'ﯿ', 'ﺉ', 'ﺊ', 'ﺋ', 'ﺌ', 'ﻯ', 'ﻰ', 'ﻱ', 'ﻲ', 'ﻳ', 'ﻴ','ئ'};
            replace = 'ی';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            find = new char[] { 'ٴ', '۽', 'ﺀ' };
            replace = 'ء';
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(find[i], replace);
            }

            string[] findString;
            string[] replaceString;
            findString = new string[] { "ﻵ", "ﻶ", "ﻷ", "ﻸ", "ﻹ", "ﻺ", "ﻻ", "ﻼ" };
            replaceString = new string[] { "لا" };
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(findString[i], replaceString[0]);
            }

            findString = new string[]{ "ﷲ", "﷼", "ﷳ", "ﷴ", "ﷵ", "ﷶ", "ﷷ", "ﷸ",
                "ﷹ", "ﷺ", "ﷻ" };
            replaceString = new string[] { "الله", "ریال", "اکبر", "محمد", "صلعم", "رسول", "علیه", "وسلم",
                "صلی", "صلی الله علیه وسلم", "جل جلاله"};
            for (int i = 0; i < find.Length; i++)
            {
                text = text.Replace(findString[i], replaceString[i]);
            }

            return text.MergeInsideSpaces().Trim();
        }

        /// <summary>
        /// Fixed multiform persion letters and numbers
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FixedPersianAndNumbersLetters(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = FixedNumbersToEn(text);

            return FixPersianLetters(text);
        }

        /// <summary>
        /// change arabic or persian numbers to english numbers
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FixedNumbersToEn(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = PerToEngNumbers(text);
            text = ArToEngNumbers(text);

            return text;
        }

        public static string MergeInsideSpaces(this string text)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            return regex.Replace(text, " ");
        }
    }
}