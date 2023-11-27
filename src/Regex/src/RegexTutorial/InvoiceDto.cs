using System.Text.Json.Serialization;

namespace RegexTutorial;

// 表示请求开发票的 Dto 对象
public class InvoiceRequestDto
{
    // 发票抬头类型 （1-个人，2-企业）
    public int InvoiceTitleType { get; set; }

    // 发票抬头名称
    public string? InvoiceTitle { get; set; }

    //纳税人识别号（企业抬头必填）
    public string? TaxpayerRegistrationNumber { get; set; }

    // 开户银行名称
    public string? BankName { get; set; }

    //   银行账户
    public string? bankAccount { get; set; }

    // 注册地址
    public string? registration_address { get; set; }

    // 注册电话
    public string? RegistrationPhoneNumber { get; set; }

    // 注册邮箱
    public string? REGISTRATION_EMAIL { get; set; }

    // 发票明细列表
    public List<InvoiceItemDto>? InvoiceItems { get; set; }
}

/// <summary>
///     发票明细 Dto
/// </summary>
public class InvoiceItemDto
{
    /// <summary>
    ///     商品名称
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    ///     商品数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    ///     商品单价（单位：分）
    /// </summary>
    public int UnitPrice { get; set; }

    /// <summary>
    ///     商品税率（6%、9%、13%）
    /// </summary>
    public decimal TaxRate { get; set; }
}
