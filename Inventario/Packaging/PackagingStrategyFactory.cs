namespace Inventario.Packaging
{
    public static class PackagingStrategyFactory
    {
        public static IPackagingStrategy ObtenerEstrategia(string profileCode)
        {
            if (string.IsNullOrWhiteSpace(profileCode))
            {
                return new DefaultStrategy();
            }

            switch (profileCode.Trim())
            {
                case "10":
                    return new LevisStrategy();

                case "1000":
                    return new CintasStrategy();

                case "77":
                    return new ZumiesStrategy();

                case "88":
                    return new TargetsStrategy();

                case "55":
                    return new BeansStrategy();

                case "66":
                    return new RockyStrategy();

                case "44":
                    return new OldNavyStrategy();

                case "33":
                    return new SaftechStrategy();

                case "22":
                    return new IfrStrategy();

                default:
                    return new DefaultStrategy();
            }
        }
    }
}