using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Payments.BahamtaIR;

[assembly: PluginInfo(
    FriendlyName = "Bahamta.ir",
    Group = "Payment methods",
    SystemName = BahamtaIRPaymentDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "Amir Hosein Aghajani",
    Version = "1.01"
)]