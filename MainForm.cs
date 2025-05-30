using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using dotenv.net;

namespace Proyecto_Final_1
{
    public partial class Mainform : Form
    {
        /* ───── Campos ───── */
        private readonly GroqClient _groq;
        private readonly BusContext _db;

        public Mainform()
        {
            InitializeComponent();

            DotEnv.Load(new DotEnvOptions(probeForEnv: true, envFilePaths: new[] {
        Path.Combine(Application.StartupPath, ".env")
    }));

            string apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("ERROR: No se encontró GROQ_API_KEY en .env");
                Application.Exit(); 
                return;
            }

            _groq = new GroqClient(apiKey);
            _db = new BusContext("Server=LAPTOP-0VH2N7ML\\SQLEXPRESS01;Database=ProyectoFINAL2;Trusted_Connection=True;");
        }


        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string origen = cbmOrigen.Text.Trim();
            string destino = cbmDestino.Text.Trim();

            if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
            {
                txtRespuestaIA.Text = "Debes seleccionar origen y destino.";
                return;
            }

            DataTable horarios = _db.ObtenerHorarios(origen, destino);
            gridHorarios.DataSource = horarios;

            if (horarios.Rows.Count == 0)
            {
                txtRespuestaIA.Text = "No se encontraron horarios disponibles para esta ruta.";
                return;
            }

            StringBuilder sbHorarios = new StringBuilder();
            foreach (DataRow row in horarios.Rows)
            {
                sbHorarios.AppendLine($"Salida: {row["HoraSalida"]} - Llegada: {row["HoraLlegada"]}");
            }
            txtRespuestaIA.Text = sbHorarios.ToString();
            AnalizarDisponibilidadBuses(origen, destino);


            try
            {
                var tiempos = ObtenerTiempoJutiapa(origen, destino);
                decimal precioAprox = CalcularPrecioAproximado(tiempos.normal);

                StringBuilder promptBuilder = new StringBuilder();
                if (enIngles)
                {
                    promptBuilder.AppendLine($"Recommend the best schedule to travel between {origen} and {destino} in Jutiapa.");
                    promptBuilder.AppendLine($"Estimated time: {tiempos.normal / 60} hours without traffic, {tiempos.conTrafico / 60} hours with traffic.");
                    promptBuilder.AppendLine("\nAvailable schedules:");
                    promptBuilder.AppendLine($"Estimated fare: Q{precioAprox} (based on travel time).");
                    // Instrucciones en INGLÉS
                    promptBuilder.AppendLine("Please answer in a clear, concise, and structured way.");
                    promptBuilder.AppendLine("Use bullet points or numbered lists. Avoid unnecessary repetition.");

                    foreach (DataRow row in horarios.Rows)
                    {
                        promptBuilder.AppendLine($"- {row["HoraSalida"]} to {row["HoraLlegada"]} ({row["Disponibilidad"]})");
                    }
                    promptBuilder.AppendLine("\nConsider:");
                    promptBuilder.AppendLine("- Rush hours: 6:30-8:00 AM and 5:00-6:30 PM");
                    promptBuilder.AppendLine("- Routes known for sharp curves");
                    promptBuilder.AppendLine("- Prefer morning schedules for better visibility");
                }
                else
                {
                    promptBuilder.AppendLine($"Recomienda el mejor horario para viajar entre {origen} y {destino} en Jutiapa.");
                    promptBuilder.AppendLine($"Tiempo estimado: {tiempos.normal / 60} horas sin tráfico, {tiempos.conTrafico / 60} horas con tráfico.");
                    promptBuilder.AppendLine("\nHorarios disponibles:");
                    promptBuilder.AppendLine($"Tarifa estimada: Q{precioAprox} (según el tiempo de viaje).");
                    // Instrucciones en ESPAÑOL
                    promptBuilder.AppendLine("Por favor, responde de forma clara, concisa y estructurada.");
                    promptBuilder.AppendLine("Utiliza viñetas o listas numeradas. Evita repeticiones innecesarias.");

                    foreach (DataRow row in horarios.Rows)
                    {
                        promptBuilder.AppendLine($"- {row["HoraSalida"]} a {row["HoraLlegada"]} ({row["Disponibilidad"]})");
                    }
                    promptBuilder.AppendLine("\nConsidera:");
                    promptBuilder.AppendLine("- Horas pico: 6:30-8:00 AM y 5:00-6:30 PM");
                    promptBuilder.AppendLine("- Rutas conocidas con curvas pronunciadas");
                    promptBuilder.AppendLine("- Preferir horarios matutinos para mejor visibilidad");
                }

                string prompt = promptBuilder.ToString();
                string respuesta = await _groq.ChatAsync(prompt);

                txtRespuestaIA.AppendText("\n\n--- RECOMENDACIÓN ---\n");
                txtRespuestaIA.AppendText(respuesta);

                _db.GuardarConsulta(origen, destino, prompt, respuesta);
            }
            catch (Exception ex)
            {
                txtRespuestaIA.AppendText($"\n\nError al consultar la IA: {ex.Message}");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            cbmOrigen.Items.AddRange(new string[] {
        "Jutiapa",
    });


            cbmDestino.Items.AddRange(new string[] {
        "Jutiapa", "Agua Blanca", "Asunción Mita", "Atescatempa",
        "Comapa", "Conguaco", "El Adelanto", "El Progreso",
        "Jalpatagua", "Jerez", "Moyuta", "Pasaco",
        "Quesada", "San José Acatempa", "Santa Catarina Mita",
        "Yupiltepeque", "Zapotitlán", "Jalapa", "Monjas"
    });
        }

        private (int normal, int conTrafico) ObtenerTiempoJutiapa(string origen, string destino)
        {
            var tiempos = new Dictionary<string, int>
    {
        {"Jutiapa-Agua Blanca", 45},
        {"Jutiapa-Asunción Mita", 60},
        {"Jutiapa-Atescatempa", 50},
        {"Jutiapa-Jalapa", 120},
        {"Asunción Mita-Santa Catarina Mita", 30},
    };

            string ruta = $"{origen}-{destino}";
            string rutaInversa = $"{destino}-{origen}";

            if (tiempos.ContainsKey(ruta))
            {
                int tiempoNormal = tiempos[ruta];
                return (tiempoNormal, (int)(tiempoNormal * 1.25)); 
            }
            else if (tiempos.ContainsKey(rutaInversa))
            {
                int tiempoNormal = tiempos[rutaInversa];
                return (tiempoNormal, (int)(tiempoNormal * 1.25));
            }

            return (90, 120); 
        }


        public class BusContext
        {
            private readonly string _conn;
            public BusContext(string conn) => _conn = conn;

   
            public DataTable ObtenerHorarios(string origen, string destino)
            {
                using var cn = new SqlConnection(_conn);
                using var cmd = new SqlCommand(@"
        SELECT HoraSalida, HoraLlegada, 
               CASE 
                   WHEN Capacidad - Pasajeros > 10 THEN 'Suficientes lugares'
                   WHEN Capacidad - Pasajeros > 0 THEN 'Pocos lugares'
                   ELSE 'Lleno'
               END AS Disponibilidad
        FROM Horarios h
        JOIN Rutas r ON h.IdRuta = r.IdRuta
        WHERE r.Origen = @o AND r.Destino = @d", cn);
                

                cmd.Parameters.AddWithValue("@o", origen);
                cmd.Parameters.AddWithValue("@d", destino);

                using var da = new SqlDataAdapter(cmd);
                var table = new DataTable();
                da.Fill(table);
                return table;
            }


            public void GuardarConsulta(string origen, string destino, string prompt, string respuesta)
            {
                using var cn = new SqlConnection(_conn);
                cn.Open();
                using var cmd = new SqlCommand(@"
            INSERT INTO Consultas(Origen, Destino, Prompt, Respuesta)
            VALUES (@o, @d, @p, @r)", cn);

                cmd.Parameters.AddWithValue("@o", origen);
                cmd.Parameters.AddWithValue("@d", destino);
                cmd.Parameters.AddWithValue("@p", prompt);
                cmd.Parameters.AddWithValue("@r", respuesta);
                cmd.ExecuteNonQuery();
            }
        }

        private async void btnBuscarHora_Click(object sender, EventArgs e)
        {
            if (gridHorarios.SelectedRows.Count == 0)
            {
                txtRespuestaIA.Text = "Debes seleccionar un horario en la tabla.";
                return;
            }

            var row = gridHorarios.SelectedRows[0];
            string horaSalida = row.Cells["HoraSalida"].Value.ToString();
            string horaLlegada = row.Cells["HoraLlegada"].Value.ToString();
            string disponibilidad = row.Cells["Disponibilidad"].Value.ToString();

            string origen = cbmOrigen.Text.Trim();
            string destino = cbmDestino.Text.Trim();

            if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
            {
                txtRespuestaIA.Text = "Debes seleccionar origen y destino.";
                return;
            }

            try
            {
                var tiempos = ObtenerTiempoJutiapa(origen, destino);
                decimal precioAprox = CalcularPrecioAproximado(tiempos.normal);


                StringBuilder promptBuilder = new StringBuilder();
                promptBuilder.AppendLine($"Analiza el siguiente horario para viajar entre {origen} y {destino} en Jutiapa:");
                promptBuilder.AppendLine($"Tiempo estimado: {tiempos.normal / 60} horas sin tráfico, {tiempos.conTrafico / 60} horas con tráfico.");
                promptBuilder.AppendLine("\nCosas a condierar al llegar al destino sinedo pasajerod e bus:");
                promptBuilder.AppendLine("- Horas pico: 6:30-8:00 AM  y 12:00PM a 14:00PM");
                promptBuilder.AppendLine($"Tarifa estimada: Q{precioAprox} (según el tiempo de viaje).");
                promptBuilder.AppendLine("- Preferir horarios matutinos para mejor visibilidad");
                promptBuilder.AppendLine("Por favor, responde de forma clara, concisa y estructurada.");
                promptBuilder.AppendLine("Utiliza viñetas o listas numeradas. Evita repeticiones innecesarias.");



                string prompt = promptBuilder.ToString();
                string respuesta = await _groq.ChatAsync(prompt);

                txtRespuestaIA.Text = $"--- RECOMENDACIÓN PARA {horaSalida} ---\n{respuesta}";

                _db.GuardarConsulta(origen, destino, prompt, respuesta);
            }
            catch (Exception ex)
            {
                txtRespuestaIA.Text = $"Error al consultar la IA: {ex.Message}";
            }
        }

        private void txtRespuestaIA_TextChanged(object sender, EventArgs e)
        {
            gridHorarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridHorarios.MultiSelect = false;
        }

        private void btnTraducir_Click(object sender, EventArgs e)
        {
            enIngles = true;
            this.Text = "Bus Schedules Jutiapa";
            btnBuscar.Text = "Search";
            btnBuscarHora.Text = "AI Recommendation";
            btnTraducir.Text = "Translate to English";
            btnEspanol.Text = "Back to Spanish";
            label1.Text = "Origin";
            label2.Text = "Destination";
      
            cbmOrigen.Items.Clear();
            cbmOrigen.Items.AddRange(new string[] { "Jutiapa" });

            cbmDestino.Items.Clear();
            cbmDestino.Items.AddRange(new string[] {
        "Jutiapa", "Agua Blanca", "Asunción Mita", "Atescatempa",
        "Comapa", "Conguaco", "El Adelanto", "El Progreso",
        "Jalpatagua", "Jerez", "Moyuta", "Pasaco",
        "Quesada", "San José Acatempa", "Santa Catarina Mita",
        "Yupiltepeque", "Zapotitlán", "Jalapa", "Monjas"
    });

            if (gridHorarios.Columns.Count > 0)
            {
                gridHorarios.Columns["HoraSalida"].HeaderText = "Departure";
                gridHorarios.Columns["HoraLlegada"].HeaderText = "Arrival";
                gridHorarios.Columns["Disponibilidad"].HeaderText = "Availability";
            }

        
        }
        private void AnalizarDisponibilidadBuses(string origen, string destino)
        {
            var ahora = DateTime.Now.TimeOfDay;
            DataTable horarios = _db.ObtenerHorarios(origen, destino);

            var busesDisponibles = horarios.AsEnumerable()
                .Where(row => TimeSpan.Parse(row["HoraSalida"].ToString()) > ahora)
                .OrderBy(row => TimeSpan.Parse(row["HoraSalida"].ToString()))
                .ToList();

            if (busesDisponibles.Count > 0)
            {
                var proximo = busesDisponibles.First();
                txtRespuestaIA.Text = $"El próximo bus directo a {destino} sale a las {proximo["HoraSalida"]}.";
            }
            else
            {
                // Buscar rutas alternativas, por ejemplo, pasando por Quesada
                DataTable horariosQuesada = _db.ObtenerHorarios(origen, "Quesada");
                var busesAQuesada = horariosQuesada.AsEnumerable()
                    .Where(row => TimeSpan.Parse(row["HoraSalida"].ToString()) > ahora)
                    .OrderBy(row => TimeSpan.Parse(row["HoraSalida"].ToString()))
                    .ToList();

                if (busesAQuesada.Count > 0)
                {
                    txtRespuestaIA.Text = $"El bus directo a {destino} ya salió hoy. " +
                        $"Puedes tomar un bus a Quesada a las {busesAQuesada.First()["HoraSalida"]} y de ahí otro a {destino}.";
                }
                else
                {
                    txtRespuestaIA.Text = $"No hay más buses directos ni alternativos disponibles hoy para {destino}.";
                }
            }
        }
        private void btnEspanol_Click(object sender, EventArgs e)
        {
            enIngles = false;
            this.Text = "Horarios de Buses Jutiapa";
            btnBuscar.Text = "Buscar";
            btnBuscarHora.Text = "Recomendación IA";
            btnTraducir.Text = "Traducir a Inglés";
            btnEspanol.Text = "Volver al Español";
            label1.Text = "Origen";
            label2.Text = "Destino";
            

            cbmOrigen.Items.Clear();
            cbmOrigen.Items.AddRange(new string[] { "Jutiapa" });

            cbmDestino.Items.Clear();
            cbmDestino.Items.AddRange(new string[] {
        "Jutiapa", "Agua Blanca", "Asunción Mita", "Atescatempa",
        "Comapa", "Conguaco", "El Adelanto", "El Progreso",
        "Jalpatagua", "Jerez", "Moyuta", "Pasaco",
        "Quesada", "San José Acatempa", "Santa Catarina Mita",
        "Yupiltepeque", "Zapotitlán", "Jalapa", "Monjas"
    });

            if (gridHorarios.Columns.Count > 0)
            {
                gridHorarios.Columns["HoraSalida"].HeaderText = "Hora de Salida";
                gridHorarios.Columns["HoraLlegada"].HeaderText = "Hora de Llegada";
                gridHorarios.Columns["Disponibilidad"].HeaderText = "Disponibilidad";
            }
        }
        private decimal CalcularPrecioAproximado(int minutos)
{

    decimal precio = 5m;
    if (minutos > 30)
    {
        int minutosExtra = minutos - 30;
        precio += (decimal)Math.Ceiling(minutosExtra / 10.0) * 2m;
    }
    if (precio > 45m)
        precio = 45m;
    return precio;
}

    }
}
