namespace ApiPos.Context
{
        public interface IPosContext
        {
            void SetEmpresa(string clientId, string empresa);
            string? GetEmpresa(string clientId);
        }

        public class PosContext : IPosContext
        {
            private static readonly Dictionary<string, string> _empresaPorCliente = new();

            public void SetEmpresa(string clientId, string empresa)
            {
                _empresaPorCliente[clientId] = empresa;
            }

            public string? GetEmpresa(string clientId)
            {
                return _empresaPorCliente.TryGetValue(clientId, out var empresa) ? empresa : null;
            }
        }
}
