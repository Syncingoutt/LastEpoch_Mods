namespace LastEpoch_Hud.Tests.Interop;

/// <summary>One mod call that writes a game constant.</summary>
/// <param name="Caller"><c>Type::Method</c> that makes the call.</param>
/// <param name="Setter"><c>DeclaringType::set_X</c> that is called.</param>
internal sealed record ConstWrite(string Caller, string Setter);
