using Data.Articulos;
using Data.Bitacora;
using Data.Cierre;
using Data.Cliente;
using Data.Endpoint;
using Data.Surtidor;
using ApiPos.Context;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
 .ConfigureApiBehaviorOptions(options =>
  {
      options.SuppressModelStateInvalidFilter = true;
  });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPosContext, PosContext>();

builder.Services.AddSingleton<StartupService>();

#region Repositorios
#endregion


/**
 * Ruta> api/pos/parametros
 * Tipo: {GET}
 * Parametros: numero de surtidor
 * Retorno: Retorna los parametros iniciales para poder facturar y ligar la empresa correspondiente
 */
builder.Services.AddScoped<ParametrosRepository>();//listo
/**
 * Ruta> api/pos/pisteros
 * Tipo: {GET}
 * Parametros: numero de surtidor
 * Retorno: Retorna una lista de todos los pisteros activos en el sistema
 */
builder.Services.AddScoped<PisteroRepository>();//listo
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: ----
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<FechaTurnoRepository>();//lsito
/**
 * Ruta> api/pos/tipo-cambio
 * Tipo: {GET}
 * Parametros: fecha, compra
 * Retorno: Retorna el tipo de cambio con la fecha correspondiente y el monto de cls/$

 */
builder.Services.AddScoped<TipoCambioRepository>();
/**
 * Ruta> api/pos/iva
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la tasa de IVA actual del sistema
 */
builder.Services.AddScoped<IVARepository>();//listo
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<ValidaUsuarioRepository>();//listo
/**
 * Ruta> api/pos/bitacora/descarga
 * Tipo: {Post}
 * Parametros: codRecurso,  noFactura,  cantSuper (double),  iniSuper (double),  FinSuper (double), cantPlus (double), iniPlus (double), FinPlus (double), cantDiesel (double), iniDiesel (double), finDiesel (double), notas
 * Retorno: Registra la bitácora de descarga (cantidades y lecturas por combustible).

 */
builder.Services.AddScoped<BitacoraDescargasRepository>();//lsito

/**
 * Ruta> api/pos/bitacora/medidas
 * Tipo: {PUT}
 * Parametros: codRecurso, cantSuper (double), cantPlus91 (double), cantDiesel (double), fechaCorte
 * Retorno: Registra las medidas de los tanques por producto por corte

 */
builder.Services.AddScoped<IngresarMedidaTanqueRepository>();//lsito

builder.Services.AddScoped<FacturaRepository>();//listo




#region Aceites
/**
 * Ruta> api/pos/articulos/lista
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la lista de los productos activos en el sistema con su id y su precio
 */
builder.Services.AddScoped<ListaArticulosRepository>(); //listo
/**
 * Ruta> api/pos/articulos/venta
 * Tipo: {GET}
 * Parametros: codRecurso
 * Retorno: Retorna un reporte de las ventas de aceite realizadas por el recurso dividido en nombre del producto y cantidad vendida

 */
builder.Services.AddScoped<VentasAceiteRepository>();//listo
/**
 * Ruta> api/pos/articulos/inventario
 * Tipo: {POST}
 * Parametros: codRecurso, registro (‘C’ o ‘R’), List<String>(LidArticulos), List<String>(cantArticulos)
 * Retorno: Registra una reposicion o cierre de aceites
 */
builder.Services.AddScoped<InventarioArticulosRepository>(); //listo
#endregion

#region Calibraciones
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<CalibracionRepository>(); //listo
builder.Services.AddScoped<IngresaCalibracionRepository>();//listo

#endregion

#region Cierre
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<CierreResumenRepository>();//listo
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<VersionRepository>();
/**
 * Ruta> api/pos/cierre/resumen
 * Tipo: {GET}
 * Parametros: codRecurso
 * Retorno:  Retorna fecha, version y lista de los valores necesarios para registrar un cierre de ventas
 *  
 * Ruta> api/pos/cierre
 * Tipo: {POST}
 * Parametros: codRecurso
 * Retorno:  Realiza el cierre del dia. Lleva varios pasos: Primero, verifica que fecha natural > fecha turno y hora > 4am. Si se cumple, entonces registra un cierre de aceites con todos los artículos  (Get ListaArticulos) con cantidad en 0. Luego, realiza el cierre en el controlador. Por ultimo, cambia la fecha de turno.

 */
builder.Services.AddScoped<CierreRepository>(); //listo
/**
 * Ruta> api/pos/cierre/limite
 * Tipo: {GET}
 * Parametros: codRecurso
 * Retorno: Verifica si el pistero puede realizar cierre
 */
builder.Services.AddScoped<LimiteCierresRepository>(); //listo
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<TurnoNocturnoRepository>(); //listo

#endregion

#region Cliente
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<ClienteRepository>(); //listo
/**
 * Ruta> api/pos/cliente
 * Tipo: {GET}
 * Parametros: codCliente,  nombre,  telefono,  correo,  tipoCedula, numeroCedula
 * Retorno: Crea un cliente nuevo con los datos indicados. 
 */
builder.Services.AddScoped<RegistraClienteRepository>();//lsito
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<ConsultaClienteRepository>();//listo
/**
 * Ruta> api/pos/cliente/correo
 * Tipo: {PUT}
 * Parametros: codCliente, correo
 * Retorno: Actualiza el correo electronico del cliente
 */
builder.Services.AddScoped<ActualizaCorreoClienteRepository>(); //listo
/**
 * Ruta> api/pos/cliente/lista
 * Tipo: {GET}
 * Parametros: subText
 * Retorno: Retorna el codigo y nombre del cliente que coincide con la busqueda. Si no hay un resultado exacto, retorna todos los clientes con alguna similitud.
 */
builder.Services.AddSingleton<ListaClienteRepository>();//lsito
/**
 * Ruta> api/pos/cliente/placas
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna una lista con las placas y un identificador si requiere tag para ser utilizada
 */
builder.Services.AddScoped<CargaPlacasClienteRepository>(); //listo
/**
 * Ruta> api/pos/cliente/codigo
 * Tipo: {GET}
 * Parametros: tipoCedula, numeroCedula
 * Retorno: Consulta si la cedula existe en el sistema. De existir retorna la informacion del cliente existente. De no existir, retorna el nombre y el nuevo codigo para crear el cliente (usar api hacienda para el nombre
 */
builder.Services.AddScoped<SiguienteCodigoRepository>();//listo
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<PlacaRepository>();//listo
/**
 * Ruta> api/pos/cliente/tag
 * Tipo: {PUT}
 * Parametros: codRecurso, codCliente, placa, tag
 * Retorno: Registra un tag de un vehiculo para su aprobacion y activacion

 */
builder.Services.AddScoped<RegistrarTagRepository>();//listo
/**
 * Ruta> api/pos/cliente/datos
 * Tipo: {GET}
 * Parametros:  tag o codCliente (Ojo solo puede recibir 1. Hay q validar**)
 * Retorno: Consulta los datos del cliente para facturar. Se puede consultar por codCliente o tag. Si se consulta por tag, la respuesta incluye la placa
 */
builder.Services.AddScoped<CargaDatosClienteRepository>(); //listo

builder.Services.AddScoped<CedulaRepository>(); //listo

builder.Services.AddScoped<ActividadEconomicaRepository>(); //listo

#endregion

#region Despachos
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<DespachoLibreRepository>();//listo
/**
 * Ruta> api/pos/despacho/agrupar-despachos
 * Tipo: {GET}
 * Parametros: pSurtidor, pRecurso
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<AgruparDespachosRepository>();//listo
/**
 * Ruta> api/pos/surtidor/anula
 * Tipo: {Post}
 * Parametros: codRecurso, surtidor
 * Retorno: Solicita anular la factura correspondiente al ultimo despacho del surtidor indicado
 */
builder.Services.AddScoped<AnulaSurtidorRepository>();//listo
/**
 * Ruta> api/pos/surtidor/autoriza
 * Tipo: {Post}
 * Parametros: codRecurso, surtidor
 * Retorno: Solicita autorizar un surtidor para iniciar un nuevo despacho
 */
builder.Services.AddScoped<SurtidorAutorizaRepository>();//listo
/**
 * Ruta> api/pos/surtidor/historico
 * Tipo: {GET}
 * Parametros: codRecurso, surtidor
 * Retorno: Retorna el historico de los ultimos 10 despachos realizados por el surtidor
 */
builder.Services.AddScoped<SurtidorHistoricoRepository>();//listo
/**
 * Ruta> api/pos/fecha-turno
 * Tipo: {GET}
 * Parametros: --
 * Retorno: Retorna la fecha actual del turno en curso
 */
builder.Services.AddScoped<LecturaRepository>();//listo

builder.Services.AddScoped<SurtidorFacturaRepository>();//listo

/**
 * Ruta> /api/pos/..
 * Tipo: {GET}
 * Parametros: numero de surtidor
 * Retorno: Lista de los surtidores existentes en la estacion
 * 
 * Tipo: {POST}
 * Parametros: numero de surtidor
 * Retorno: Lista de los surtidores existentes en la estacion
 */
builder.Services.AddScoped<ListadoSurtidorRepository>();

#endregion






var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var startupService = scope.ServiceProvider.GetRequiredService<StartupService>();//listo
    await startupService.InicializarEmpresaAsync();
}



    app.UseSwagger();
    app.UseSwaggerUI();


app.UseAuthorization();
app.MapControllers();
app.Run();

