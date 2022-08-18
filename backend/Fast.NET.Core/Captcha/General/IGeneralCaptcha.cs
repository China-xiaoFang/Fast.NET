namespace Fast.NET.Core.Captcha.General;

public interface IGeneralCaptcha
{
    dynamic CheckCode(GeneralCaptchaInput input);
    string CreateCaptchaImage(int length = 4);
}