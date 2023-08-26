namespace Bookify.web.Core.Const
{
    public class Error
    {
        public const string max = " Length can not be more than {1} ";
        public const string Dublicated = " {0} with the same name is already exist";
        public const string DublicatedBooks = " book with the same title and author is already exits";
        public const string extensition = "the website to not support the image extensition try png jpg jpeg";
        public const string maxsize = "the max size for an image is 2MB";
        public const string EditionNum = "Edition Number should be in between 1 and 1000";
        public const string dataError = "you can not choose a date form the future";
        public const string Email_O_Usrname = "A user with the same {0} is already exited";
        public const string Username_validation= "User name can only have character or numbers";
        public const string OnlyEnglishLetter= "only english letters";
        public const string NumberValidation= "This number is not valid ";
        public const string nationalIdValidation= "this national Id in not Valid";
        public const string phoneNumbers= "this phone number in not Valid";
        public const string image= "Image Field is required";

        public const string password = "that passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least eight characters long.";
    }
}
