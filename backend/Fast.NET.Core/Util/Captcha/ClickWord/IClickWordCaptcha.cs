namespace Fast.NET.Core.Util.Captcha.ClickWord;

public interface IClickWordCaptcha
{
    Task<dynamic> CheckCode(ClickWordCaptchaInput input);
    Task<ClickWordCaptchaResult> CreateCaptchaImage(string code, int width, int height);
    string RandomCode(int number);
}