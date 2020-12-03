using Datos;
using Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using System.Data;
using System.Collections;
namespace Negocio
{
    public class N_PET
    {
        D_PET datos = new D_PET();
        D_PAR_TIP_CARACTER log = new D_PAR_TIP_CARACTER();

        public string Ejecuta(string respuesta, string id_Ejec_1)
        {
            /*****************SE CREA EL ID DE EJECUCION*****************/
            string id_Ejec = log.Get_fecha_x_numero();
            id_Ejec = id_Ejec.Replace(",", "");

            log.eLog("Se Inicia las Transacciones  - Id Ejecución: " + id_Ejec, id_Ejec_1 + "_" + id_Ejec);
            string resp = ListarTransacciones(respuesta, id_Ejec_1 + "_" + id_Ejec);
            log.eLog("Se Terminan las Transacciones  - Id Ejecución: " + id_Ejec, id_Ejec_1 + "_" + id_Ejec);
            log.eLog("**************************************************************", id_Ejec_1 + "_" + id_Ejec);
            return resp;
        }

        public string ListarTransacciones(string respuesta, string id_Ejec)
        {
            D_PAR_TIP_CARACTER log = new D_PAR_TIP_CARACTER();
            List<E_PET> lista = new List<E_PET>();
            List<E_PET_DATO> listaTrans = new List<E_PET_DATO>();

            try
            {
                /**************BUSCAR TRANSACCIONES CON ESTADO PENDIENTE VALIDAR*****************/
                listaTrans = datos.ObtenerTransacciones();

                /*************SI NO EXISTEN TRANSACCIONES, SE GUARDA ERROR EN LOG**********/
                if (!listaTrans.Any())
                {
                    log.eLog("SIN TRANSACCIONES  PARA PROCESAR - Id Ejecución: " + id_Ejec, id_Ejec);
                    return "finalizado";
                }


                /*************PROCESAR UNA A UNA CADA TRANSACCION***********/
                Parallel.ForEach(listaTrans, (idTrans, pls, index) =>
                {
                    int resp = 0;
                    resp = procesarTransaccion(idTrans, id_Ejec);


                });

                //int cont = 1;
                //foreach (E_PET_DATO PET in listaTrans)
                //{
                //    int resp = 0;

                //    resp = procesarTransaccion(PET, id_Ejec + "-" + cont.ToString());
                //    cont = cont + 1;

                //}

                return "finalizado";
            }
            /*************SI SE CAE AL PROCESAR SE GUARDA ERROR EN LOG***************/
            catch (Exception ex)
            {
                log.eLog("Se Genera Excepcion en la Ejecución - Metodo ListarTransacciones: " + ex.ToString() + " - Id Ejecución: " + id_Ejec, id_Ejec);
                log.eLog("**************************************************************", id_Ejec);
                return "finalizado";
            }
        }

        public int procesarTransaccion(E_PET_DATO PET, string id_Ejec)
        {
            D_PAR_TIP_CARACTER log = new D_PAR_TIP_CARACTER();
            List<E_PAR_TIP_CARACTER> listaCaracteres = new List<E_PAR_TIP_CARACTER>();
            List<E_PET_VALIDATE> listaValidate = new List<E_PET_VALIDATE>();
            List<string> listaCaracteresOriginales = new List<string>();
            string listaStringCaracteresOriginales = "";
            string transaccion = "";
            int estadoCicloErroneo = 0;
            int cantidadTablaError = 0;
            int lineaError = 0;
            int posicionError = 0;
            bool corregido = false;
            bool corregidoIndividual = false;
            bool contieneInvalidos = false;
            bool corregidoOriginal = false;
            string cadenaReemplazo = "";
            string cadenaOriginal = "";
            string cadenaCorregir = "";
            int idPadre = 0;
            //string idPadre = "";
            byte[] registroCorregido = null;
            string mensajeError = "";
            int idFile = 0;
            string mensajeLog = "";
            string messageError = "";
            int codigoEstadoPeticion = 0;
            try
            {
                /******CODIGO ESTADO VALIDANDO*****/
                codigoEstadoPeticion = datos.obtenerEstadoPeticion(7);
                /**************CAMBIAR ESTADO TRANSACCION A VALIDANDO*************/
                while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                {
                    mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO CAMBIAR EL ESTADO A VALIDANDO, REVISE EL METODO actualizarEstadoTransaccion DE D";
                    log.eLog(mensajeLog, id_Ejec);
                }

                /*************OBTENER LISTADO DE FILES DE LA TRANSACCION***********/
                List<E_PET_FILE> listaFiles = datos.obtenerArchivos(PET);

                /*********************************************CICLO CORRECTO **************************************************************************/

                /**************RECORRER LOS ARCHIVOS DE LA TRANSACCION***************/
                if (listaFiles.Count != 0)
                {

                    for (int contFiles = 0; contFiles < listaFiles.Count; contFiles++)
                    {
                        /**************OBTENER EL ID DEL ARCHIVO***************/
                        idFile = listaFiles.ElementAt(contFiles).idPET_FILE;

                        /****************BUSCAR LA CANTIDAD DE VECES QUE EL ARCHIVO ESTA PRESENTE EN LA TABLA ERRORES**************/
                        cantidadTablaError = datos.obtenerRegistrosTablaError(idFile, PET.transaccion);

                        /*******************SI ESTA MENOS DE 3 VECES EN LA TABLA ERRORES, SE CONTINUA******************/
                        if (cantidadTablaError < 3)
                        {
                            /***************GENERAR REGISTRO DEL ARCHIVO XML, DEVUELVE 1 SI INGRESA NUEVO REGISTRO O SI EL REGISTRO YA EXISTE, 0 SI NO INGRESA REGISTRO*************/
                            if (datos.InsertarRegistro(idFile,PET.transaccion) == 1)
                            {

                                listaCaracteres = datos.ObtenerTipCaracter();

                                listaValidate = datos.ObtenerValidate(idFile,PET.transaccion);
                                listaCaracteresOriginales = datos.ObtenerTipCaracterOriginal();

                                cadenaOriginal = listaValidate.ElementAt(0).PET_VALIDATE_REGISTRO_ORIGINAL.ToString();
                                cadenaCorregir = listaValidate.ElementAt(0).PET_VALIDATE_REGISTRO_ORIGINAL.ToString();
                                cadenaReemplazo = cadenaOriginal;

                                /**************VALIDA SI EL XML ESTA CORRECTO ANTES DE CORREGIR, SI NO TIENE ERRORES SE AVANZA HASTA CAMBIAR ESTADO REGITRO**********/
                                try
                                {

                                    XmlDataDocument xmldoc1 = new XmlDataDocument();
                                    xmldoc1.PreserveWhitespace = true;
                                    xmldoc1.LoadXml(cadenaOriginal);
                                    /**********SE CONVIERTE EL XML A ARREGLO DE BYTE EN BASE 64*************/
                                    registroCorregido = null;
                                    Encoding encoding = Encoding.UTF8;
                                    registroCorregido = encoding.GetBytes(cadenaOriginal);
                                    if (registroCorregido != null)
                                    {
                                        corregido = true;
                                        corregidoOriginal = true;
                                    }
                                    /*****************ERROR SI NO PUDO SER CODIFICADO****************/
                                    else
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO CODIFICAR LA CADENA ORIGINAL, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                        log.eLog(mensajeLog, id_Ejec);
                                        mensajeError =  " NO SE PUDO CODIFICAR LA CADENA ORIGINAL, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                        while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                        {
                                            mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                            log.eLog(mensajeLog, id_Ejec);
                                        }
                                        corregido = false;

                                    }

                                }

                                /***********OBTIENE LA LINEA DEL ERROR, Y EL MENSAJE************/
                                catch (System.Xml.XmlException ex2)
                                {
                                    corregido = false;
                                    lineaError = ex2.LineNumber;
                                    posicionError = ex2.LinePosition;
                                    messageError = ex2.Message;
                                    corregidoOriginal = false;


                                }


                                catch (Exception ex2)
                                {
                                    corregido = false;
                                    corregidoOriginal = false;

                                }

                                cadenaCorregir = cadenaOriginal;


                                contieneInvalidos = false;
                                /****************SI LA FACTURA ESTA ERRONEA************/
                                if (corregido == false)
                                {
                                    /***SE PASARA 3 VECES POR LINEA, LA PRIMERA SIEMPRE ENTRA AL REEMPLAZAR TODOS LOS CARACTERES, LA 2 Y 3 A LOS SIMBOLOS < Y "**/
                                    bool paso3veces = false;
                                    /*SE CORRIGE DE LAS  3 FORMAS , SI NO FUNCIONA NINGUNA DE LAS 3 SIGNIFICA QUE NO SE PUEDE*/
                                    while (corregido != true && paso3veces == false)
                                    {
                                        int paso = 0;
                                        corregidoIndividual = false;

                                        /****CORRIGE DE LAS TRES FORMAS**/
                                        while (corregidoIndividual != true && paso < 3)
                                        {

                                            string cadenaAux = cadenaCorregir;

                                            /***************CORREGIR EL RESTO DE SIMBOLOS  LOS ACTIVOS, CORRIGE TODOS  Y PASA SOLO 1 VEZ POR AQUI*******************/
                                            if (corregidoIndividual != true && contieneInvalidos == false)
                                            {
                                                /***********BUSCA SI EXISTEN CARACTERES NO VALIDOS EN EL STRING VALIDATE_ORIGINAL, LOS CON ESTADO A**********/
                                                for (int contCaracteres = 0; contCaracteres < listaCaracteres.Count; contCaracteres++)
                                                {

                                                    string caracterOriginal = listaCaracteres.ElementAt(contCaracteres).PAR_TIP_CARACTER_ORIGINAL.ToString().Trim();
                                                    string caracterReemplazo = listaCaracteres.ElementAt(contCaracteres).PAR_TIP_CARACTER_REEMPLAZO.ToString().Trim();
                                                    listaCaracteresOriginales.Add(caracterOriginal);
                                                    listaStringCaracteresOriginales = listaStringCaracteresOriginales + caracterOriginal;

                                                    /***********SI SE ENCUENTRA UN CARACTER NO VALIDO SE REEMPLAZA POR EL CARACTER VALIDO QUE LE CORRESPONDE**********/
                                                    if (cadenaAux.Contains(caracterOriginal) == true)

                                                    {
                                                        contieneInvalidos = true;
                                                        cadenaAux = cadenaAux.Replace(caracterOriginal, caracterReemplazo);
                                                    }
                                                }

                                                if (cadenaAux != cadenaCorregir)
                                                {
                                                    cadenaCorregir = cadenaAux;
                                                    contieneInvalidos = true;
                                                    corregidoIndividual = true;
                                                }
                                            }

                                            /***************CORREGIR EL SIMBOLO <  *******************/
                                            if (corregidoIndividual != true)
                                            {
                                                cadenaAux = reemplazarMenorQue(cadenaAux, lineaError, idFile);
                                                if (cadenaAux != cadenaCorregir)
                                                {
                                                    cadenaCorregir = cadenaAux;
                                                    corregidoIndividual = true;
                                                }
                                            }

                                            /***************CORREGIR EL SIMBOLO "  *******************/
                                            if (corregidoIndividual != true)
                                            {
                                                cadenaAux = reemplazarComillas(cadenaAux, lineaError, idFile);
                                                if (cadenaAux != cadenaCorregir)
                                                {
                                                    cadenaCorregir = cadenaAux;
                                                    corregidoIndividual = true;
                                                }
                                            }
                                            paso = paso + 1;

                                        }


                                        /************COMPROBAR SI HAY MAS ERRORES, SI NO  HAY SE TRANSFORMA A BASE64***********/
                                        if (corregidoIndividual == true)
                                        {
                                            corregido = false;
                                            cadenaReemplazo = cadenaCorregir;
                                            XmlDataDocument xmldoc = new XmlDataDocument();
                                            xmldoc.PreserveWhitespace = true;

                                            /***********SE GENERA EL XML CON EL REGISTRO CADENA STRING CON LOS CARACTERES VALIDOS***************/
                                            try
                                            {
                                                xmldoc.LoadXml(cadenaReemplazo);
                                                /**********SE CONVIERTE EL XML A ARREGLO DE BYTE EN BASE 64*************/
                                                registroCorregido = null;
                                                Encoding encoding = Encoding.UTF8;
                                                registroCorregido = encoding.GetBytes(cadenaReemplazo);
                                                if (registroCorregido != null)
                                                {
                                                    corregido = true;
                                                }
                                                /*****************ERROR SI NO PUDO SER CODIFICADO****************/
                                                else
                                                {
                                                    mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO CODIFICAR, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                                    log.eLog(mensajeLog, id_Ejec);
                                                    mensajeError =  " NO SE PUDO CODIFICAR, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                                    while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                                    {
                                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D_4002";
                                                        log.eLog(mensajeLog, id_Ejec);
                                                    }
                                                    corregido = false;
                                                }
                                            }
                                            catch (System.Xml.XmlException ex)
                                            {
                                                corregido = false;
                                                lineaError = ex.LineNumber;
                                                posicionError = ex.LinePosition;
                                                messageError = ex.Message;

                                            }

                                            catch (Exception ex)
                                            {
                                                corregido = false;
                                            }

                                        }
                                        /*****AUMENTA EL CONTADOR CADA VEZ QUE VALIDA EL XML, SON 3 VECES POR LINEA******/
                                        if (paso >= 3)
                                        {
                                            paso3veces = true;
                                        }

                                    }
                                }
                                /****************SE CAMBIA EL ESTADO DEL REGISTRO A CORREGIDO**********/
                                if (corregido == true && registroCorregido != null)
                                {

                                    /***********ACTUALIZAR REGISTRO REEMPLAZO************/
                                    while (datos.actualizarRegistroValidate(idFile, cadenaReemplazo,PET.transaccion) != 1)
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO REEMPLAZAR EL NUEVO REGISTRO EN LA TABLA VALIDATE, REVISE EL METODO actualizarRegistroValidat EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                    }

                                    /*********ACTUALIZAR ESTADO DEL REGISTRO A CORREGIDO***************/
                                    while (datos.actualizarEstadoRegistro(idFile, 1, PET.transaccion) != 1)
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO ACTUALIZAR EL ESTADO A CORREGIDO EN LA TABLA VALIDATE, REVISE EL METODO ctualizarEstadoRegistro EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                    }

                                    /*****SI EL ORIGINAL ESTA CORRECTO NO ACTUALIZA EL REGISTRO****/
                                    if (corregidoOriginal != true)

                                    {
                                        if (datos.actualizarArchivo(idFile, registroCorregido, PET.transaccion) == 1)
                                        {
                                            while (datos.actualizarEstadoTablaError(idFile, 1, PET.transaccion) != 1)
                                            {
                                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO ACTUALIZAR A ESTADO CORREGIDO EN LA TABLA VALIDATE_FILE_ERR, REVISE EL METODO actualizarEstadoTablaError EN D";
                                                log.eLog(mensajeLog, id_Ejec);
                                            }

                                        }
                                        /**********ERROR AL ACTUALIZAR REGISTRO EN TABLA **************/
                                        else
                                        {

                                            mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO REEMPLAZAR EL REGISTRO NUEVO  EN LA TABLA , REVISE EL METODO datos.actualizarArchivo EN D";
                                            log.eLog(mensajeLog, id_Ejec);
                                            mensajeError =  " NO SE PUDO REEMPLAZAR EL REGISTRO NUEVO  EN LA TABLA , REVISE EL METODO datos.actualizarArchivo EN D";
                                            while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                            {
                                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D_4002";
                                                log.eLog(mensajeLog, id_Ejec);
                                            }
                                        }
                                    }
                                }
                                /*****************ERROR SI NO ES CORREGIDO****************/
                                else
                                {
                                    mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile +  messageError;
                                    log.eLog(mensajeLog, id_Ejec);
                                    mensajeError =   messageError;
                                    while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                    }

                                }

                            }
                            /***************ERROR AL GENERAR REGISTRO*****************/
                            else
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO GENERAR/INSERTAR EL REGISTRO EN LA TABLA VALIDATE, REVISE EL METODO InsertarRegistro EN D";
                                log.eLog(mensajeLog, id_Ejec);
                                mensajeError =  " NO SE PUDO GENERAR/INSERTAR EL REGISTRO EN LA TABLA VALIDATE, REVISE EL METODO InsertarRegistro EN D";
                                while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                {
                                    mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                    log.eLog(mensajeLog, id_Ejec);
                                }
                            }
                        }

                        /***********ERROR SI EL ARCHIVO APARECE 3 O MAS VECES EN LA TABLA ERRORES, SE CAMBIA EL ESTADO DE  LA TRANSACCION A ERRONEA**********/
                        else
                        {
                            mensajeLog = " ERROR TRANSACCION: " + PET + " LA TRANSACCION ES INCORREGIBLE: REGISTRO" + idFile + " ESTA PRESENTE 3 O MAS VECES EN LA TABLA ERRORES";
                            log.eLog(mensajeLog, id_Ejec);
                            mensajeError =  " LA TRANSACCION ES INCORREGIBLE: EL REGISTRO" + idFile + " ESTA PRESENTE 3 O MAS VECES EN LA TABLA ERRORES";
                            while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                log.eLog(mensajeLog, id_Ejec);
                            }

                            //CAMBIO TRANSACCION, ERRONEA
                            /******CODIGO ESTADO ERRONEO*****/
                            codigoEstadoPeticion = datos.obtenerEstadoPeticion(5);
                            while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO ACTUALIZAR EL ESTADO DE LA TRANSACCION A ERRONEO, REVISE EL METODO actualizarEstadoTransaccion EN D";
                                log.eLog(mensajeLog, id_Ejec);
                            }
                            idPadre = Int32.Parse(datos.obtenerIdPadre(PET));
                            while (datos.actualizarEstadoIdPadre(idPadre,PET.transaccion, 5) != 1)
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LAS TRANSACCIONES CON MISMO ID PADRE A ERRONEA, REVISE EL METODO actualizarEstadoIdPadre EN D";
                                log.eLog(mensajeLog, id_Ejec);
                            }
                            WS_Notificaciones_Web_Cliente.WS_Notificaciones_Web_Cliente ws = new WS_Notificaciones_Web_Cliente.WS_Notificaciones_Web_Cliente();
                            ws.Timeout = -1;
                            WS_Notificaciones_Web_Cliente.CABECERA_Type CABECERA_WSEMAIL = new WS_Notificaciones_Web_Cliente.CABECERA_Type();
                            WS_Notificaciones_Web_Cliente.DATOS_Type DATOS_WSEMAIL = new WS_Notificaciones_Web_Cliente.DATOS_Type();

                            /******SE INICIA EL WS_Acceso Y SE DECLARA SUS TIPO DE DATOS PARA ENVIAR Y RECIBIR******/
                            WS_Notificaciones_Web_Cliente.MENSAJERES_Type MENSAJERES_WSEMAIL = new WS_Notificaciones_Web_Cliente.MENSAJERES_Type();
                            CABECERA_WSEMAIL.APP_CONSUMIDORA = "APP VALIDATE";
                            DATOS_WSEMAIL.idPadre = datos.obtenerIdPadre(PET);
                            MENSAJERES_WSEMAIL = ws.Error_Validacion(CABECERA_WSEMAIL, DATOS_WSEMAIL);
                            if (MENSAJERES_WSEMAIL.INTEGRES.DETALLE != null && MENSAJERES_WSEMAIL.INTEGRES.DETALLE.DATOS.resultCode == "1")
                            {
                                return 0;
                            }
                            else
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ENVIAR EL EMAIL INDICANDO QUE LA TRANSACCION, TIENE UN ARCHIVO ERRONEO, REVISE EL WS_NOTIFICACIONES_WEBCLIENTE Y SUS DATOS DE ENVIO";
                                log.eLog(mensajeLog, id_Ejec);
                                return 0;
                            }
                        }
                    }
                }

                // }
                /**************ERROR SI TRANSACCION NO TIENE ARCHIVOS**************/
                else
                {
                    //SE GUARDA MENSAJE EN TABLA ERROR, idFile ES CERO POR DEFECTO
                    mensajeLog = " ERROR TRANSACCION: " + PET + " NO TIENE ARCHIVOS, REVISE TABLA PET_FILE";
                    log.eLog(mensajeLog, id_Ejec);
                    mensajeError = " LA TRANSACCION NO TIENE ARCHIVOS, REVISE TABLA PET_FILE";
                    while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                    {
                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO GUARDAR EL ERROR EN TABLA ERRORES,REVISE EL METODO GuardarError EN D";
                        log.eLog(mensajeLog, id_Ejec);
                    }
                    /******CODIGO ESTADO ERRONEO*****/
                    codigoEstadoPeticion = datos.obtenerEstadoPeticion(5);
                    while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                    {
                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LA TRANSACCION A ERRONEO, REVISE EL METODO actualizarEstadoTransaccion EN D";
                        log.eLog(mensajeLog, id_Ejec);
                    }
                    return 0;
                }

                /*********************************************TERMINA CICLO CORRECTO******************************************************************/


                /****************************PREGUNTA SI EN LA TRANSACCION HAY ALGUN ARCHIVO ERRONEO***/
                if (datos.obtenerErroresTransaccion(PET) == 0)
                {
                    /********************CAMBIAR ESTADO TRANSACCION A PENDIENTE, ESTO PASA CUANDO NO HAY NINGUN ARCHIVO ERRONEO EN  LA TRANSACCION*/
                    /******CODIGO ESTADO PENDIENTE*****/
                    codigoEstadoPeticion = datos.obtenerEstadoPeticion(0);
                    while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                    {
                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LA TRANSACCION A PENDIENTE DE VALIDAR,REVISE EL METODO actualizarEstadoTransaccion EN D";
                        log.eLog(mensajeLog, id_Ejec);
                    }
                    /*********CAMBIO ID PADRE TRANSACCIONES, PENDIENTE VALIDAR***************/
                    idPadre = Int32.Parse(datos.obtenerIdPadre(PET));
                    while (datos.actualizarEstadoIdPadre(idPadre, PET.transaccion, 0) != 1)
                    {
                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LAS TRANSACCIONES CON MISMO ID PADRE A PENDIENTE VALIDAR, REVISE EL METODO actualizarEstadoIdPadre EN D";
                        log.eLog(mensajeLog, id_Ejec);
                    }
                    mensajeLog = " TRANSACCION: " + PET + " SE EJECUTO CORRECTAMENTE, TODOS LOS ARCHIVOS SE CORRIGIERON";
                    log.eLog(mensajeLog, id_Ejec);
                    return 1;
                }

                /********************************************************CICLO ERRONEO************************************************************/

                else
                {
                    /**EMPIEZA CON ESTADO DEL CICLO ERRONEO 1, AL TERMINAR  SE CAMBIA A 0**/
                    estadoCicloErroneo = 1;
                    while (estadoCicloErroneo == 1)
                    {
                        /*************OBTENER LISTADO DE FILES DE LA TRANSACCION***********/
                        List<E_PET_FILE> listaErroneos = datos.obtenerRegistrosErroneos(PET);

                        /**************CICLO CORRECTO *****************/
                        /**************RECORRER LOS ARCHIVOS DE LA TRANSACCION***************/
                        if (listaErroneos.Count != 0)
                        {
                            for (int contErroneo = 0; contErroneo < listaErroneos.Count; contErroneo++)
                            {
                                /**************OBTENER EL ID DEL ARCHIVO***************/
                                idFile = listaErroneos.ElementAt(contErroneo).idPET_FILE;

                                /****************BUSCAR LA CANTIDAD DE VECES QUE EL ARCHIVO ESTA PRESENTE EN LA TABLA ERRORES**************/
                                cantidadTablaError = datos.obtenerRegistrosTablaError(idFile, PET.transaccion);

                                /*******************SI ESTA MENOS DE 3 VECES EN LA TABLA ERRORES, SE CONTINUA******************/
                                if (cantidadTablaError < 3)
                                {
                                    /***************GENERAR REGISTRO DEL ARCHIVO XML, DEVUELVE 1 SI INGRESA NUEVO REGISTRO O SI EL REGISTRO YA EXISTE, 0 SI NO INGRESA REGISTRO*************/
                                    if (datos.InsertarRegistro(idFile,PET.transaccion) == 1)
                                    {

                                        listaCaracteres = datos.ObtenerTipCaracter();

                                        listaValidate = datos.ObtenerValidate(idFile,PET.transaccion);
                                        listaCaracteresOriginales = datos.ObtenerTipCaracterOriginal();

                                        cadenaOriginal = listaValidate.ElementAt(0).PET_VALIDATE_REGISTRO_ORIGINAL.ToString();
                                        cadenaCorregir = listaValidate.ElementAt(0).PET_VALIDATE_REGISTRO_ORIGINAL.ToString();
                                        cadenaReemplazo = cadenaOriginal;

                                        /**************VALIDA SI EL XML ESTA CORRECTO ANTES DE REEMPLAZAR CARACTERES, SI NO TIENE ERRORES SE AVANZA HASTA CAMBIAR ESTADO REGITRO**********/
                                        try
                                        {

                                            XmlDataDocument xmldoc1 = new XmlDataDocument();
                                            xmldoc1.PreserveWhitespace = true;
                                            xmldoc1.LoadXml(cadenaOriginal);
                                            /**********SE CONVIERTE EL XML A ARREGLO DE BYTE EN BASE 64*************/
                                            registroCorregido = null;
                                            Encoding encoding = Encoding.UTF8;
                                            registroCorregido = encoding.GetBytes(cadenaOriginal);
                                            if (registroCorregido != null)
                                            {
                                                corregido = true;
                                                corregidoOriginal = true;
                                            }
                                            /*****************ERROR SI NO PUDO SER CODIFICADO****************/
                                            else
                                            {
                                                mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO CODIFICAR LA CADENA ORIGINAL, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                                log.eLog(mensajeLog, id_Ejec);
                                                mensajeError =  " NO SE PUDO CODIFICAR LA CADENA ORIGINAL, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                                while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                                {
                                                    mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                                    log.eLog(mensajeLog, id_Ejec);
                                                }
                                                corregido = false;

                                            }

                                        }

                                        /*OBTIENE LA LINEA DE ERROR Y EL MENSAJA*/
                                        catch (System.Xml.XmlException ex2)
                                        {
                                            corregido = false;
                                            lineaError = ex2.LineNumber;
                                            posicionError = ex2.LinePosition;
                                            messageError = ex2.Message;

                                        }


                                        catch (Exception ex2)
                                        {
                                            corregido = false;
                                        }

                                        cadenaCorregir = cadenaOriginal;

                                        contieneInvalidos = false;



                                        /****************SI LA FACTURA ESTA ERRONEA************/
                                        if (corregido == false)
                                        {
                                            /***SE PASARA 3 VECES POR LINEA, LA PRIMERA SIEMPRE ENTRA AL REEMPLAZAR TODOS LOS CARACTERES, LA 2 Y 3 A LOS SIMBOLOS < Y "**/
                                            bool paso3veces = false;
                                            while (corregido != true && paso3veces == false)
                                            {
                                                int paso = 0;
                                                corregidoIndividual = false;

                                                /**VALIDA DE LAS 3 FORMAS**/
                                                while (corregidoIndividual != true && paso < 3)
                                                {

                                                    string cadenaAux = cadenaCorregir;

                                                    /***************CORREGIR EL RESTO DE SIMBOLOS  LOS ACTIVOS, CORRIGE TODOS  Y PASA SOLO 1 VEZ POR AQUI*******************/
                                                    if (corregidoIndividual != true && contieneInvalidos == false)
                                                    {
                                                        /***********BUSCA SI EXISTEN CARACTERES NO VALIDOS EN EL STRING VALIDATE_ORIGINAL, LOS CON ESTADO A**********/
                                                        for (int contCaracteres = 0; contCaracteres < listaCaracteres.Count; contCaracteres++)
                                                        {

                                                            string caracterOriginal = listaCaracteres.ElementAt(contCaracteres).PAR_TIP_CARACTER_ORIGINAL.ToString().Trim();
                                                            string caracterReemplazo = listaCaracteres.ElementAt(contCaracteres).PAR_TIP_CARACTER_REEMPLAZO.ToString().Trim();
                                                            listaCaracteresOriginales.Add(caracterOriginal);
                                                            listaStringCaracteresOriginales = listaStringCaracteresOriginales + caracterOriginal;

                                                            /***********SI SE ENCUENTRA UN CARACTER NO VALIDO SE REEMPLAZA POR EL CARACTER VALIDO QUE LE CORRESPONDE**********/
                                                            if (cadenaAux.Contains(caracterOriginal) == true)

                                                            {
                                                                contieneInvalidos = true;
                                                                cadenaAux = cadenaAux.Replace(caracterOriginal, caracterReemplazo);
                                                            }
                                                        }

                                                        if (cadenaAux != cadenaCorregir)
                                                        {
                                                            cadenaCorregir = cadenaAux;
                                                            contieneInvalidos = true;
                                                            corregidoIndividual = true;
                                                        }
                                                    }

                                                    /***************CORREGIR EL SIMBOLO <  *******************/
                                                    if (corregidoIndividual != true)
                                                    {
                                                        cadenaAux = reemplazarMenorQue(cadenaAux, lineaError, idFile);
                                                        if (cadenaAux != cadenaCorregir)
                                                        {
                                                            cadenaCorregir = cadenaAux;
                                                            corregidoIndividual = true;
                                                        }
                                                    }

                                                    /***************CORREGIR EL SIMBOLO "  *******************/
                                                    if (corregidoIndividual != true)
                                                    {
                                                        cadenaAux = reemplazarComillas(cadenaAux, lineaError, idFile);
                                                        if (cadenaAux != cadenaCorregir)
                                                        {
                                                            cadenaCorregir = cadenaAux;
                                                            corregidoIndividual = true;
                                                        }
                                                    }
                                                    paso = paso + 1;

                                                }


                                                /************COMPROBAR SI HAY MAS ERRORES, SI NO  HAY SE TRANSFORMA A BASE64***********/
                                                if (corregidoIndividual == true)
                                                {
                                                    corregido = false;
                                                    cadenaReemplazo = cadenaCorregir;
                                                    XmlDataDocument xmldoc = new XmlDataDocument();
                                                    xmldoc.PreserveWhitespace = true;

                                                    /***********SE GENERA EL XML CON EL REGISTRO CADENA STRING CON LOS CARACTERES VALIDOS***************/
                                                    try
                                                    {
                                                        xmldoc.LoadXml(cadenaReemplazo);
                                                        /**********SE CONVIERTE EL XML A ARREGLO DE BYTE EN BASE 64*************/
                                                        registroCorregido = null;
                                                        Encoding encoding = Encoding.UTF8;
                                                        registroCorregido = encoding.GetBytes(cadenaReemplazo);
                                                        if (registroCorregido != null)
                                                        {
                                                            corregido = true;
                                                        }
                                                        /*****************ERROR SI NO PUDO SER CODIFICADO****************/
                                                        else
                                                        {
                                                            mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO CODIFICAR, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                                            log.eLog(mensajeLog, id_Ejec);
                                                            mensajeError =  " NO SE PUDO CODIFICAR, REVISE SI AL encoding.GetBytes SE LE PASA UNA CADENA NO NULA";
                                                            while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                                            {
                                                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D_4002";
                                                                log.eLog(mensajeLog, id_Ejec);
                                                            }
                                                            corregido = false;
                                                        }
                                                    }
                                                    catch (System.Xml.XmlException ex)
                                                    {
                                                        corregido = false;
                                                        lineaError = ex.LineNumber;
                                                        posicionError = ex.LinePosition;
                                                        messageError = ex.Message;

                                                    }

                                                    catch (Exception ex)
                                                    {
                                                        corregido = false;
                                                    }

                                                }

                                                /**A**UMENTA EL CONTADOR, PASA 3 VECES***/
                                                if (paso >= 3)
                                                {
                                                    paso3veces = true;
                                                }

                                            }
                                        }
                                        /****************SE CAMBIA EL ESTADO DEL REGISTRO A CORREGIDO**********/
                                        if (corregido == true && registroCorregido != null)
                                        {

                                            /***********ACTUALIZAR REGISTRO REEMPLAZO************/
                                            while (datos.actualizarRegistroValidate(idFile, cadenaReemplazo, PET.transaccion) != 1)
                                            {
                                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO REEMPLAZAR EL NUEVO REGISTRO EN LA TABLA VALIDATE, REVISE EL METODO actualizarRegistroValidat EN D";
                                                log.eLog(mensajeLog, id_Ejec);
                                            }

                                            /*********ACTUALIZAR ESTADO DEL REGISTRO A CORREGIDO***************/
                                            while (datos.actualizarEstadoRegistro(idFile, 1, PET.transaccion) != 1)
                                            {
                                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO ACTUALIZAR EL ESTADO A CORREGIDO EN LA TABLA VALIDATE, REVISE EL METODO ctualizarEstadoRegistro EN D";
                                                log.eLog(mensajeLog, id_Ejec);
                                            }


                                            if (corregidoOriginal == false)

                                            {
                                                if (datos.actualizarArchivo(idFile, registroCorregido, PET.transaccion) == 1)
                                                {
                                                    while (datos.actualizarEstadoTablaError(idFile, 1, PET.transaccion) != 1)
                                                    {
                                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO ACTUALIZAR A ESTADO CORREGIDO EN LA TABLA VALIDATE_FILE_ERR, REVISE EL METODO actualizarEstadoTablaError EN D";
                                                        log.eLog(mensajeLog, id_Ejec);
                                                    }

                                                }
                                                /**********ERROR AL ACTUALIZAR REGISTRO EN TABLA **************/
                                                else
                                                {

                                                    mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO REEMPLAZAR EL REGISTRO NUEVO  EN LA TABLA , REVISE EL METODO datos.actualizarArchivo EN D";
                                                    log.eLog(mensajeLog, id_Ejec);
                                                    mensajeError =  " NO SE PUDO REEMPLAZAR EL REGISTRO NUEVO  EN LA TABLA , REVISE EL METODO datos.actualizarArchivo EN D";
                                                    while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                                    {
                                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D_4002";
                                                        log.eLog(mensajeLog, id_Ejec);
                                                    }
                                                }
                                                //datos.actualizarEstadoTransaccion(PET, 5);
                                            }
                                        }
                                        /*****************ERROR SI NO ES CORREGIDO****************/
                                        else
                                        {
                                            mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile +  messageError;
                                            log.eLog(mensajeLog, id_Ejec);
                                            mensajeError =   messageError;
                                            while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                            {
                                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                                log.eLog(mensajeLog, id_Ejec);
                                            }

                                        }

                                    }
                                    /***************ERROR AL GENERAR REGISTRO*****************/
                                    else
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " EL REGISTRO: " + idFile + " NO SE PUDO GENERAR/INSERTAR EL REGISTRO EN LA TABLA VALIDATE, REVISE EL METODO InsertarRegistro EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                        mensajeError =  " NO SE PUDO GENERAR/INSERTAR EL REGISTRO EN LA TABLA VALIDATE, REVISE EL METODO InsertarRegistro EN D";
                                        while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                        {
                                            mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                            log.eLog(mensajeLog, id_Ejec);
                                        }
                                    }
                                }

                                /***********ERROR SI EL ARCHIVO APARECE 3 O MAS VECES EN LA TABLA ERRORES, SE CAMBIA EL ESTADO DE  LA TRANSACCION A ERRONEA**********/
                                else
                                {
                                    mensajeLog = " ERROR TRANSACCION: " + PET + " LA TRANSACCION ES INCORREGIBLE: REGISTRO:" + idFile + " ESTA PRESENTE 3 O MAS VECES EN LA TABLA ERRORES";
                                    log.eLog(mensajeLog, id_Ejec);
                                    mensajeError =  " LA TRANSACCION ES INCORREGIBLE: EL REGISTRO:" + idFile + " ESTA PRESENTE 3 O MAS VECES EN LA TABLA ERRORES";
                                    while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                    }

                                    /*********CAMBIO TRANSACCION, ERRONEA***************/
                                    /******CODIGO ESTADO ERRONEA*****/
                                    codigoEstadoPeticion = datos.obtenerEstadoPeticion(5);
                                    while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LA TRANSACCION A ERRONEA, REVISE EL METODO actualizarEstadoTransaccion EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                    }

                                    /*********CAMBIO ID PADRE TRANSACCIONES, ERRONEA***************/
                                    idPadre = Int32.Parse(datos.obtenerIdPadre(PET));
                                    while (datos.actualizarEstadoIdPadre(idPadre,PET.transaccion, 5) != 1)
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LAS TRANSACCIONES CON MISMO ID PADRE A ERRONEA, REVISE EL METODO actualizarEstadoIdPadre EN D";
                                        log.eLog(mensajeLog, id_Ejec);
                                    }

                                    /******SE INICIA EL WS_NOTIFICACIONES_EMAIL PARA ENVIAR AVISO DE ERRORES******/
                                    WS_Notificaciones_Web_Cliente.WS_Notificaciones_Web_Cliente ws = new WS_Notificaciones_Web_Cliente.WS_Notificaciones_Web_Cliente();
                                    ws.Timeout = -1;
                                    WS_Notificaciones_Web_Cliente.CABECERA_Type CABECERA_WSEMAIL = new WS_Notificaciones_Web_Cliente.CABECERA_Type();
                                    WS_Notificaciones_Web_Cliente.DATOS_Type DATOS_WSEMAIL = new WS_Notificaciones_Web_Cliente.DATOS_Type();

                                    WS_Notificaciones_Web_Cliente.MENSAJERES_Type MENSAJERES_WSEMAIL = new WS_Notificaciones_Web_Cliente.MENSAJERES_Type();
                                    CABECERA_WSEMAIL.APP_CONSUMIDORA = "APP VALIDATE";
                                    DATOS_WSEMAIL.idPadre = datos.obtenerIdPadre(PET);
                                    MENSAJERES_WSEMAIL = ws.Error_Validacion(CABECERA_WSEMAIL, DATOS_WSEMAIL);
                                    if (MENSAJERES_WSEMAIL.INTEGRES.DETALLE != null && MENSAJERES_WSEMAIL.INTEGRES.DETALLE.DATOS.resultCode =="1") {
                                        return 0;
                                    }
                                    else
                                    {
                                        mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ENVIAR EL EMAIL INDICANDO QUE LA TRANSACCION, TIENE UN ARCHIVO ERRONEO, REVISE EL WS_NOTIFICACIONES_WEBCLIENTE Y SUS DATOS DE ENVIO";
                                        log.eLog(mensajeLog, id_Ejec);
                                        return 0;
                                    }
                                }
                            }
                        }



                        /**************ERROR SI TRANSACCION NO TIENE ARCHIVOS**************/
                        else
                        {
                            /**************SE GUARDA MENSAJE EN TABLA ERROR, idFile ES CERO POR DEFECTO*******/
                            mensajeLog = " ERROR TRANSACCION: " + PET + " NO TIENE ARCHIVOS ,REVISE TABLA PET_FILE";
                            log.eLog(mensajeLog, id_Ejec);
                            mensajeError = "ERROR TRANSACCION: " + PET + " NO TIENE ARCHIVOS,REVISE TABLA PET_FILE";
                            while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                log.eLog(mensajeLog, id_Ejec);
                            }
                            /******CODIGO ESTADO ERRONEA*****/
                            codigoEstadoPeticion = datos.obtenerEstadoPeticion(5);
                            while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                            {
                                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D";
                                log.eLog(mensajeLog, id_Ejec);
                            }
                            return 0;
                        }

                    }

                    if (datos.obtenerErroresTransaccion(PET) == 0)
                    {
                        /********************CAMBIAR ESTADO TRANSACCION A PENDIENTE, ESTO PASA CUANDO NO HAY NINGUN ARCHIVO ERRONEO EN  LA TRANSACCION*/
                        /******CODIGO ESTADO PENDIENTE*****/
                        codigoEstadoPeticion = datos.obtenerEstadoPeticion(0);
                        while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 0)
                        {
                            mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LA TRANSACCION A PENDIENTE DE VALIDAR,REVISE EL METODO actualizarEstadoTransaccion EN D";
                            log.eLog(mensajeLog, id_Ejec);
                        }
                        /*********CAMBIO ID PADRE TRANSACCIONES, PENDIENTE VALIDAR***************/
                        idPadre = Int32.Parse(datos.obtenerIdPadre(PET));
                        while (datos.actualizarEstadoIdPadre(idPadre,PET.transaccion, 0) != 1)
                        {
                            mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LAS TRANSACCIONES CON MISMO ID PADRE A PENDIENTE VALIDAR, REVISE EL METODO actualizarEstadoIdPadre EN D";
                            log.eLog(mensajeLog, id_Ejec);
                        }
                        estadoCicloErroneo = 0;
                        mensajeLog = " TRANSACCION: " + PET + " SE EJECUTO CORRECTAMENTE, TODOS LOS ARCHIVOS SE CORRIGIERON";
                        log.eLog(mensajeLog, id_Ejec);
                    }
                }

                /*********************************************TERMINA CICLO ERRONEO******************************************************************/

                return 1;


            }
            /********************************ERROR NO REGISTRADO*******************************/
            catch (Exception ex)
            {
                int guardoError = 0;
                mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO: " + idFile + " ERROR NO REGISTRADO" + ex.Message;
                log.eLog(mensajeLog, id_Ejec);
                mensajeError =  " ERROR NO REGISTRADO" + ex.Message;
                while (datos.GuardarError(idFile, PET, mensajeError, PET.transaccion) != 1)
                {
                    mensajeLog = " ERROR TRANSACCION: " + PET + " REGISTRO" + idFile + " NO SE PUDO GUARDAR ERROR EN TABLA ERRORES, REVISE EL METODO GuardarError EN D ";
                    log.eLog(mensajeLog, id_Ejec);
                }
                //CAMBIO TRANSACCION, ERRONEA
                /******CODIGO ESTADO ERRONEA*****/
                codigoEstadoPeticion = datos.obtenerEstadoPeticion(5);
                while (datos.actualizarEstadoTransaccion(PET, codigoEstadoPeticion) != 1)
                {
                    mensajeLog = " ERROR TRANSACCION: " + PET + " NO SE PUDO ACTUALIZAR EL ESTADO DE LA TRANSACCION A ERRONEA,REVISE EL METODO actualizarEstadoTransaccion EN D";
                    log.eLog(mensajeLog, id_Ejec);
                }
                return 0;

            }

        }

        /***REEMPLAZAR LOS  COMILLAS " POR LINEA**/

        public string reemplazarComillas(string cadenaOriginal, int lineaError, int idFile)
        {
            string cadenaCorregida = "";
            if (lineaError != 0 && cadenaOriginal != null)
            {
                string Directorio = Properties.Settings.Default.rutaXml;
                string ruta = System.AppDomain.CurrentDomain.BaseDirectory + Directorio + "\\";

                if (!(Directory.Exists(ruta)))
                {
                    Directory.CreateDirectory(ruta);
                }
                string fecha = DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace("-", "").Replace(":", "");


                string factura = ruta + "FACTURA_" + idFile + "_" + fecha + ".xml";


                File.WriteAllText(factura, cadenaOriginal);
                string[] lineas = System.IO.File.ReadAllLines(factura);

                if (factura != null)
                {
                    //Elimino archivos del servidor
                    if (System.IO.File.Exists(factura))
                    {
                        System.IO.File.Delete(factura);
                    }
                }

                string lineaStringOriginal = lineas[lineaError - 1];
                List<E_PAR_TIP_ELEMENTOATRIBUTOXML> listaElementoAtributo = new List<E_PAR_TIP_ELEMENTOATRIBUTOXML>();
                listaElementoAtributo = datos.ObtenerElementoAtributo();
                List<string> listaElementoAtributoPalabra = datos.ObtenerElementoAtributoPalabra();

                string posicion = "";
                string posicionOriginal = "";
                string ultimoAnterior = "";
                string primeroSiguiente = "";
                string posicionAnterior = "";
                string posicionSiguiente = "";
                string posicionSinIgual = "";
                string lineaBuena = "";


                char separador = '\"';

                String[] separarString = lineaStringOriginal.Split(separador);
                for (int i = 1; i < separarString.Length; i++)
                {
                    separarString[i] = separador + separarString[i];
                }


                for (int cont = 0; cont < separarString.Length; cont++)
                {
                    posicionOriginal = separarString[cont];
                    posicion = posicionOriginal.Replace(" ", "");


                    if (cont == 0)
                    {
                        ultimoAnterior = "";
                        if (cont + 1 < separarString.Length)
                        {
                            posicionSiguiente = separarString[cont + 1];
                            posicionSiguiente = posicionSiguiente.Replace(" ", "");
                            primeroSiguiente = posicionSiguiente.Substring(0, 1);
                        }

                    }
                    if (cont != 0)
                    {
                        posicionAnterior = separarString[cont - 1];
                        posicionAnterior = posicionAnterior.Replace(" ", "");
                        ultimoAnterior = posicionAnterior.Substring(posicionAnterior.Length - 1, 1);
                        if (cont + 1 < separarString.Length)
                        {
                            posicionSiguiente = separarString[cont + 1];
                            posicionSiguiente = posicionSiguiente.Replace(" ", "");
                            primeroSiguiente = posicionSiguiente.Substring(0, 1);
                        }
                        else
                        {
                            posicionSiguiente = "";
                            primeroSiguiente = "";
                        }
                    }
                    if (ultimoAnterior != "" && ultimoAnterior != "=" && primeroSiguiente != "" && primeroSiguiente != "<")
                    {
                        int largoPosicion = posicion.Length;
                        if (posicion.Contains("=") == true)
                        {
                            posicionSinIgual = posicion.Remove(largoPosicion - 1, 1);
                            posicionSinIgual = posicionSinIgual.Remove(0, 1);

                        }
                        else
                        {
                            posicionSinIgual = posicion;
                            posicionSinIgual = posicion.Remove(0, 1);

                        }
                        if (listaElementoAtributoPalabra.Contains(posicionSinIgual) == true)
                        {
                            lineaBuena = lineaBuena + posicionOriginal;
                        }
                        else
                        {
                            posicion = posicionOriginal;
                            lineaBuena = lineaBuena + posicion.Remove(0, 1).Insert(0, "&quot;");
                            Console.Write("error");
                        }
                    }
                    else
                    {
                        lineaBuena = lineaBuena + posicionOriginal;

                    }


                }

                cadenaCorregida = cadenaOriginal.Replace(lineaStringOriginal, lineaBuena);
                return cadenaCorregida;

            }
            else
            {
                return cadenaOriginal;
            }

        }


        /***REEMPLAZAR LOS  SIGNOS < POR LINEA**/
        public string reemplazarMenorQue(string cadenaOriginal, int lineaError, int idFile)
        {
            string Directorio = Properties.Settings.Default.rutaXml;
            string ruta = System.AppDomain.CurrentDomain.BaseDirectory + Directorio + "\\";

            if (!(Directory.Exists(ruta)))
            {
                Directory.CreateDirectory(ruta);
            }
            string fecha = DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace("-", "").Replace(":", "");
            string factura = ruta + "FACTURA_" + idFile + "_" + fecha + ".xml";

            string cadenaCorregida = "";
            File.WriteAllText(factura, cadenaOriginal);
            string[] lineas = System.IO.File.ReadAllLines(factura);
            if (factura != null)
            {
                //Elimino archivos del servidor
                if (System.IO.File.Exists(factura))
                {
                    System.IO.File.Delete(factura);
                }
            }
            string lineaStringOriginal = lineas[lineaError - 1];
            List<E_PAR_TIP_ELEMENTOATRIBUTOXML> listaElementoAtributo = new List<E_PAR_TIP_ELEMENTOATRIBUTOXML>();
            listaElementoAtributo = datos.ObtenerElementoAtributo();
            List<string> listaElementoAtributoPalabra = datos.ObtenerElementoAtributoPalabra();

            string posicion = "";
            string posicionOriginal = "";
            string primeroSiguiente = "";
            string posicionSiguiente = "";
            string lineaBuena = "";
            string posicionEtiqueta = "";

            List<string> listaEtiquetas = new List<string>();
            listaEtiquetas.Add("<cac");
            listaEtiquetas.Add("<cbc");
            listaEtiquetas.Add("</cac");
            listaEtiquetas.Add("</cbc");


            char separador = '<';
            if (lineaError != 0 && cadenaOriginal != null)
            {


                String[] separarString = lineaStringOriginal.Split(separador);
                for (int i = 1; i < separarString.Length; i++)
                {
                    separarString[i] = separador + separarString[i];
                }

                for (int cont = 0; cont < separarString.Length; cont++)
                {
                    posicionOriginal = separarString[cont];
                    posicion = posicionOriginal;
                    if (cont + 1 < separarString.Length)
                    {
                        posicionSiguiente = separarString[cont + 1];
                        primeroSiguiente = posicionSiguiente.Substring(0, 1);
                    }
                    else
                    {
                        posicionSiguiente = "";
                        primeroSiguiente = "";
                    }

                    if (posicion.Contains('<') == true)
                    {
                        if (posicion.Contains('/') == true)
                        {
                            if (posicion.Length >= 5)
                            {
                                posicionEtiqueta = posicion.Substring(0, 5);
                                if (posicionEtiqueta.Contains(':'))
                                {
                                    posicionEtiqueta = posicionEtiqueta.Remove(4, 1);
                                }
                            }
                            else
                            {
                                posicionEtiqueta = "";
                            }
                        }
                        else
                        {
                            if (posicion.Length >= 4)
                            {
                                posicionEtiqueta = posicion.Substring(0, 4);
                            }
                            else
                            {
                                posicionEtiqueta = "";
                            }

                        }
                        if (listaEtiquetas.Contains(posicionEtiqueta) == true)
                        {

                            if (esPareada(posicion) == 0)
                            {
                                if (posicionSiguiente != "" && primeroSiguiente == "<")
                                {

                                    if (posicion.Contains('/') == true)
                                    {
                                        if (posicionSiguiente.Length >= 5)
                                        {
                                            posicionEtiqueta = posicionSiguiente.Substring(0, 5);
                                            if (posicionEtiqueta.Contains(':'))
                                            {
                                                posicionEtiqueta = posicionEtiqueta.Remove(4, 1);
                                            }
                                        }
                                        else
                                        {
                                            posicionEtiqueta = "";

                                        }
                                    }
                                    else
                                    {
                                        if (posicionSiguiente.Length >= 4)
                                        {
                                            posicionEtiqueta = posicionSiguiente.Substring(0, 4);
                                        }
                                        else
                                        {
                                            posicionEtiqueta = "";
                                        }

                                    }
                                    if (listaEtiquetas.Contains(posicionEtiqueta) == true)
                                    {
                                        lineaBuena = lineaBuena + posicion;
                                    }
                                    else
                                    {

                                        lineaBuena = lineaBuena + posicion;

                                    }
                                }
                                else
                                {
                                    lineaBuena = lineaBuena + posicion;

                                }
                            }
                            else
                            {
                                lineaBuena = lineaBuena + posicion;

                            }
                        }
                        else
                        {
                            lineaBuena = lineaBuena + posicion.Remove(0, 1).Insert(0, "&lt;");

                        }



                    }
                    else
                    {
                        lineaBuena = lineaBuena + posicion;

                    }
                }

                cadenaCorregida = cadenaOriginal.Replace(lineaStringOriginal, lineaBuena);

                return cadenaCorregida;



            }

            else
            {
                return cadenaOriginal;
            }
        }



        /***FUNCION PARA VALIDAR SI LA LINEA TIENE < Y > PARES***/
        public int esPareada(string linea)
        {

            string starters = "<";
            string enders = ">";
            int esPareada = 0;
            Stack stack = new Stack();
            foreach (char c in linea)
            {
                if (starters.Contains(c))
                {
                    stack.Push(c);
                }
                else if (enders.Contains(c))
                {
                    if (stack.Count > 0)
                    {

                        if (enders.IndexOf(c) == starters.IndexOf(Convert.ToChar(stack.Peek())))
                        {
                            stack.Pop();
                        }
                        else
                        {
                            esPareada = 0;
                        }
                    }
                }
            }

            if (stack.Count == 0)
            {
                esPareada = 1;
            }
            else
            {
                esPareada = 0;
            }
            return esPareada;
        }





    }
}
