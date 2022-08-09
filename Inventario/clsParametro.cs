using System.Data;

namespace Inventario
{
    public class clsParametro
    {
        private string m_Nombre;
        private object m_Valor;
        private SqlDbType m_TipoDato;
        private ParameterDirection m_Direccion;
        private int m_Tamaño;
        public string Nombre
        {

            get => m_Nombre;
            set => m_Nombre = value;
        }

        public object Valor
        {
            get => m_Valor;
            set => m_Valor = value;
        }

        public SqlDbType TipoDato
        {
            get => m_TipoDato;
            set => m_TipoDato = value;
        }


        public ParameterDirection Direccion
        {
            get => m_Direccion;
            set => m_Direccion = value;
        }


        public int Tamaño
        {
            get => m_Tamaño;
            set => m_Tamaño = value;
        }

        public clsParametro(string objNombre, object objValor)
        {
            m_Nombre = objNombre;
            m_Valor = objValor;
            m_Direccion = ParameterDirection.Input;
        }
        public clsParametro(string objNombre, object objValor, SqlDbType objTipoDato)
        {
            m_Nombre = objNombre;
            m_Valor = objValor;
            m_Direccion = ParameterDirection.Input;
            m_TipoDato = objTipoDato;
        }
        public clsParametro(string objNombre, object objValor, SqlDbType objTipoDato, ParameterDirection objDireccion, int objTamaño)
        {
            m_Nombre = objNombre;
            m_Valor = objValor;
            m_TipoDato = objTipoDato;
            m_Direccion = objDireccion;
            m_Tamaño = objTamaño;
        }

        public clsParametro()
        {
        }
    }
}