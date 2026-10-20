using Inventario.Reportes;

namespace Inventario
{
    // Cada clase apunta a su archivo Reportes\{Nombre}.rdlc (convertido del diseno DevExpress original).

    public class ReporteCaja : ReporteRdlc { public ReporteCaja() : base("ReporteCaja") { } }
    public class ReporteRocky : ReporteRdlc { public ReporteRocky() : base("ReporteRocky") { } }
    public class ReporteCajaZumines : ReporteRdlc { public ReporteCajaZumines() : base("ReporteCajaZumines") { } }
    public class ReporteCajaTargetAssorment : ReporteRdlc { public ReporteCajaTargetAssorment() : base("ReporteCajaTargetAssorment") { } }
    public class ReporteCajaIFR : ReporteRdlc { public ReporteCajaIFR() : base("ReporteCajaIFR") { } }
    public class ReporteCajaIFRMixtas : ReporteRdlc { public ReporteCajaIFRMixtas() : base("ReporteCajaIFRMixtas") { } }
    public class ReporteCajaBens : ReporteRdlc { public ReporteCajaBens() : base("ReporteCajaBens") { } }
    public class ReporteCajaBensCamisas : ReporteRdlc { public ReporteCajaBensCamisas() : base("ReporteCajaBensCamisas") { } }
    public class ReportCajaTarget : ReporteRdlc { public ReportCajaTarget() : base("ReportCajaTarget") { } }
    public class ReporteCintas : ReporteRdlc { public ReporteCintas() : base("ReporteCintas") { } }
    public class ReporteCajaTYNDALE : ReporteRdlc { public ReporteCajaTYNDALE() : base("ReporteCajaTYNDALE") { } }
    public class ReporteCajaSaftech : ReporteRdlc { public ReporteCajaSaftech() : base("ReporteCajaSaftech") { } }
    public class ReporteCajaOLDNAVY : ReporteRdlc { public ReporteCajaOLDNAVY() : base("ReporteCajaOLDNAVY") { } }
    public class ReporteAlmacen : ReporteRdlc { public ReporteAlmacen() : base("ReporteAlmacen") { } }
    public class ReporteDiario : ReporteRdlc { public ReporteDiario() : base("ReporteDiario") { } }
}
