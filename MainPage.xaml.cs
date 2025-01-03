﻿using System.Text.RegularExpressions;

namespace PerfectPayer
{
    public partial class MainPage : ContentPage
    {
        //vamos a definir la propina maxima
        private double subtotal { set; get; } = 0.0d;
        private double PorcentajePropina { set; get; } = 0; // El porcentaje que varia para pagar de propina.
        private double montoTotal { set; get; } = 0.0d; // el monto final que se debe pagar entre cada persona si se reparte
        private int numeroPersonas { set; get; } = 1; // numero de personas a pagar

        private string expresionRegularPrecio = "^\\$\\d{1,3}(\\.\\d{1,2})$";



        public MainPage()
        {
            InitializeComponent();
            CampoDefinirMonto.Text = "$0.00";
        }

        private void EntryPrecio_Completed(object sender, EventArgs e)  
        {
            string resultadoMonto = EntryPrecio.Text;
            CampoDefinirMonto.Text = $"${resultadoMonto}";
        }

        private void EntryPrecio_TextChanged(object sender, TextChangedEventArgs e)
        {
            //dentro de este punto vamos a escuchar por los cambios realizados... 
            string resultadoMonto = EntryPrecio.Text.Length == 0 ? "..." : EntryPrecio.Text;
            resultadoMonto = $"${resultadoMonto}";
            CampoDefinirMonto.Text = resultadoMonto;

            //tenemos que validar si el formato del precio es correcto. 
            if (Regex.IsMatch(resultadoMonto, this.expresionRegularPrecio))
            {
                this.subtotal = Double.Parse(resultadoMonto.Replace("$", ""));

                //mostrar en nuestra interfaz...
                lblSubTotal.Text = $"${String.Format("{0:F2}", this.subtotal)}";

                calcularSubTotal();
            }
        }

        private void BotonPropina(object sender, EventArgs e)
        {
            Button boton = sender as Button;
            string porcentaje = boton.Text;

            //vamos a calcular el monto de la propina. 
            CalcularPropina(porcentaje);

            //vamos a mostrar en nuestra interfaz este cambio.
            lblPropina.Text = $"$ {this.PorcentajePropina}";

            calcularSubTotal();
        }

        private void CalcularPropina(string porcentaje)
        {
            int cantidadMaximo = 25;
            porcentaje = porcentaje.Replace("%", "");

            this.PorcentajePropina = double.Parse(porcentaje) * cantidadMaximo;
            this.PorcentajePropina = this.PorcentajePropina / 50; // %50 <- es el maximo
        }

        private void calcularSubTotal()
        {
            //vamos a mostrar dentro de este subtotal.
            string precioSubTotal = ( this.subtotal > 0 ) ? 
                $"{this.subtotal + PorcentajePropina}" :
                $"$0.00";

            //esta parte se encarga de verificar que porcion de dinero le toca pagar cada persona
            //si es mas de una persona. 
            if ( this.numeroPersonas > 1 && this.subtotal > 0 )
            {
                double precioDividido = double.Parse(precioSubTotal) / this.numeroPersonas;
                precioSubTotal = String.Format("{0:F2}", precioDividido);
            }
            else
            {
                precioSubTotal = String.Format("{0:F2}", Double.Parse(precioSubTotal));
            }

            CampoDefinirMonto.Text = precioSubTotal;
        }

        private void btnMinus_Clicked(object sender, EventArgs e)
        {
            this.numeroPersonas--;

            if (this.numeroPersonas < 1)
            {
                this.numeroPersonas = 1;
            }

            lblPerson.Text = this.numeroPersonas.ToString();
            calcularSubTotal();
        }

        private void btnPlus_Clicked(object sender, EventArgs e)
        {
            this.numeroPersonas++;
            lblPerson.Text = this.numeroPersonas.ToString();
            calcularSubTotal();
        }

        private void SliderTip_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            //cambios realizado dentro de nuestro slider. 
            string porcentaje = $"%{SliderTip.Value}";
            CalcularPropina(porcentaje);

            //mostrar en dinero neuvamente la propina.

            lblPropina.Text = $"${String.Format("{0:F2}", this.PorcentajePropina)}";

            //vamos a calcular cueno debe pagar cada persona o solo una persona.
            calcularSubTotal();
        }
    }    

}
