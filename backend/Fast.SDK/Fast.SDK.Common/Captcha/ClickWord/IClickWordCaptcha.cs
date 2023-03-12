namespace Fast.SDK.Common.Captcha.ClickWord;

public interface IClickWordCaptcha
{
    /// <summary>
    /// 验证码验证
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ClickWordCaptchaResult> CheckCode(ClickWordCaptchaInput input);

    /// <summary>
    /// 生成验证码图片
    /// </summary>
    /// <param name="code"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    Task<ClickWordCaptchaResult> CreateCaptchaImage(string code, int width, int height);

    /// <summary>
    /// 随机绘制字符串
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    string RandomCode(int number);
}