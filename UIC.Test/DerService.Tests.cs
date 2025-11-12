using System.Buffers.Text;
using System.Formats.Asn1;

namespace UIC.Test;

public class DerServiceTests
{
    UserIdentity GetValidIdentity() => new UserIdentity()
    {
        UserId = 42,
        Username = "AsyncPro",
        Email = "dev@example.com",
        Permissions = ["read", "write"],
    };

    [Fact]
    public void Encode_WhenUserIdLessOrEqualThan0_ShouldThrowArgumentException()
    {
        var identity = new UserIdentity();

        Assert.Throws<ArgumentException>(() => DerService.EncodeUserIdentity(identity));
    }

    [Fact]
    public void Encode_WhenUserNameIsNull_ShouldThrowArgumentNullException()
    {
        var identity = GetValidIdentity();
        identity.Username = null!;
        identity.Email = null;
        identity.Permissions = null!;

        Assert.Throws<ArgumentNullException>(() => DerService.EncodeUserIdentity(identity));
    }

    [Fact]
    public void Encode_WhenPermissionsIsEmpty_ShouldThrowArgumentNullException()
    {
        var identity = GetValidIdentity();
        identity.Email = null;
        identity.Permissions = null!;

        Assert.Throws<ArgumentException>(() => DerService.EncodeUserIdentity(identity));
    }

    [Fact]
    public void Encode_ValidateEmailIsOptional()
    {
        var identity = GetValidIdentity();
        identity.Email = null;

        var der = DerService.EncodeUserIdentity(identity);
        var decoded = DerService.DecodeUserIdentity(der);

        Assert.Null(decoded.Email);
    }

    [Fact]
    public void Encode_WhenEmailIsEmpty_EmailShouldBeNull()
    {
        var identity = GetValidIdentity();
        identity.Email = "";

        var der = DerService.EncodeUserIdentity(identity);
        var decoded = DerService.DecodeUserIdentity(der);

        Assert.Null(decoded.Email);
    }

    [Fact]
    public void Decode_WhenBufferIsEmpty_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => DerService.DecodeUserIdentity([]));
    }

    [Fact]
    public void Decode_WhenBufferFillInvalidBytes_ThrowAsnContentException()
    {
        byte[] der =
        [
            0x30, 0x1E,
            0x02, 0x01, 0x2A,
            0x0C, 0x08, 0x41, 0x73, 0x79, 0x6E, 0x63, 0x50, 0x72, 0x6F,
            0x16, 0x0F, 0x64, 0x65, 0x76, 0x40, 0x65, 0x78, 0x61, 0x6D,
            0x70, 0x6C, 0x65, 0x2E, 0x63, 0x6F, 0x6D
        ];

        Assert.Throws<AsnContentException>(() => DerService.DecodeUserIdentity(der));
    }

    [Fact]
    public void ValidateEncodeAndDecodeCycle()
    {
        var identity = GetValidIdentity();

        var der = DerService.EncodeUserIdentity(identity);
        var decoded = DerService.DecodeUserIdentity(der);

        Assert.Equal(identity.UserId, decoded.UserId);
        Assert.Equal(identity.Username, decoded.Username);
        Assert.Equal(identity.Email, decoded.Email);
        Assert.True(identity.Permissions.All(x => decoded.Permissions.Contains(x)));
    }

    [Fact]
    public void Pem_ValidateHeaderAndFooter()
    {
        var prefix = "USER IDENTITY";
        var identity = GetValidIdentity();

        var pem = DerService.ToPem(DerService.EncodeUserIdentity(identity), prefix);

        var lines = pem.Split("\n");

        Assert.Contains($"BEGIN {prefix}", lines.First());
        Assert.Contains($"END {prefix}", lines.Last());
    }

    [Fact]
    public void Pem_BodyIsValidBase64()
    {
        var prefix = "USER IDENTITY";
        var identity = GetValidIdentity();

        var pem = DerService.ToPem(DerService.EncodeUserIdentity(identity), prefix);

        var lines = pem.Split("\n");

        var body = string.Join("", lines.Skip(1).Take(lines.Length - 2));

        Assert.NotEmpty(body);
        Assert.True(Base64.IsValid(body));
    }

    [Fact]
    public void Pem_IsDeConvertable()
    {
        var identity = GetValidIdentity();

        var pem = DerService.ToPem(DerService.EncodeUserIdentity(identity));
        var buffer = DerService.FromPem(pem);

        Assert.NotEmpty(buffer);
    }

    [Fact]
    public void Pem_IsDecodable()
    {
        var identity = GetValidIdentity();

        var pem = DerService.ToPem(DerService.EncodeUserIdentity(identity));
        var buffer = DerService.FromPem(pem);
        var decoded = DerService.DecodeUserIdentity(buffer);

        Assert.Equal(identity.UserId, decoded.UserId);
        Assert.Equal(identity.Username, decoded.Username);
        Assert.Equal(identity.Email, decoded.Email);
        Assert.True(identity.Permissions.All(x => decoded.Permissions.Contains(x)));
    }
}