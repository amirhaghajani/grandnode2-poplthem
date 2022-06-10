using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Payments.IDPayIR;

[assembly: PluginInfo(
    FriendlyName = "Id Pay.ir",
    Group = "Payment methods",
    SystemName = IDPayIRPaymentDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "Amir Hosein Aghajani",
    Version = "1.01"
)]