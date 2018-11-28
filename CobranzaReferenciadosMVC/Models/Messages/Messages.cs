namespace CobranzaReferenciadosMVC.Models.Messages
{
    /// <summary>
    /// Representa un mensaje con un indicador booleano de éxito.
    /// </summary>
    public class Message
    {
        public bool Status { get; protected set; }
        public string Mensaje { get; protected set; }

        public Message(bool status, string mensaje)
        {
            Status = status;
            Mensaje = mensaje;
        }
    }
}