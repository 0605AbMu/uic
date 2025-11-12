using System.Formats.Asn1;

namespace UIC;

public class DerService
{
    public static byte[] EncodeUserIdentity(UserIdentity identity)
    {
        UserIdentity.Validate(identity);

        var writer = new AsnWriter(AsnEncodingRules.DER);

        writer.PushSequence(); //start a sequence

        writer.WriteInteger(identity.UserId);
        writer.WriteCharacterString(UniversalTagNumber.UTF8String, identity.Username);

        if (string.IsNullOrEmpty(identity.Email))
            writer.WriteNull();
        else
            writer.WriteCharacterString(UniversalTagNumber.IA5String, identity.Email);

        writer.PushSequence();
        foreach (var identityPermission in identity.Permissions)
        {
            writer.WriteCharacterString(UniversalTagNumber.UTF8String, identityPermission);
        }

        writer.PopSequence();

        writer.PopSequence(); //finish a sequence

        return writer.Encode();
    }

    public static UserIdentity DecodeUserIdentity(byte[] data)
    {
        var reader = new AsnReader(data, AsnEncodingRules.DER);

        if (!reader.HasData)
            throw new InvalidOperationException("Reader has no data");

        var identity = new UserIdentity();

        var sequenceReader = reader.ReadSequence();

        if (sequenceReader.TryReadInt32(out var value))
            identity.UserId = value;

        identity.Username = sequenceReader.ReadCharacterString(UniversalTagNumber.UTF8String);

        if (sequenceReader.PeekTag().HasSameClassAndValue(Asn1Tag.Null))
            sequenceReader.ReadNull();
        else
            identity.Email = sequenceReader.ReadCharacterString(UniversalTagNumber.IA5String);
        
        identity.Permissions = new List<string>();
        var permissionsSeqReader = sequenceReader.ReadSequence();

        while (permissionsSeqReader.HasData)
        {
            identity.Permissions.Add(permissionsSeqReader.ReadCharacterString(UniversalTagNumber.UTF8String));
        }

        permissionsSeqReader.ThrowIfNotEmpty();

        reader.ThrowIfNotEmpty();
        sequenceReader.ThrowIfNotEmpty();

        UserIdentity.Validate(identity);

        return identity;
    }

    public static string ToPem(byte[] der, string label = "USER IDENTITY")
    {
        var base64 = Convert.ToBase64String(der, Base64FormattingOptions.InsertLineBreaks);
        return $"-----BEGIN {label}-----\n{base64}\n-----END {label}-----";
    }

    public static byte[] FromPem(string pem, string label = "USER IDENTITY")
    {
        var header = $"-----BEGIN {label}-----";
        var footer = $"-----END {label}-----";
        var base64 = pem
            .Replace(header, "")
            .Replace(footer, "")
            .Replace("\n", "")
            .Replace("\r", "");
        return Convert.FromBase64String(base64);
    }
}