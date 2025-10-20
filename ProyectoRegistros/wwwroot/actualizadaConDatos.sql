CREATE DATABASE  IF NOT EXISTS `proyectoregistro` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `proyectoregistro`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: proyectoregistro
-- ------------------------------------------------------
-- Server version	8.0.34

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `alumno`
--

DROP TABLE IF EXISTS `alumno`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `alumno` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(45) NOT NULL,
  `fechaCumple` datetime NOT NULL,
  `Edad` int NOT NULL,
  `Direccion` varchar(50) NOT NULL,
  `NumContacto` varchar(15) NOT NULL,
  `NumSecundario` varchar(15) NOT NULL,
  `Padecimientos` varchar(45) NOT NULL,
  `Tutor` varchar(30) NOT NULL,
  `Email` varchar(45) NOT NULL,
  `AtencionPsico` tinyint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alumno`
--

LOCK TABLES `alumno` WRITE;
/*!40000 ALTER TABLE `alumno` DISABLE KEYS */;
INSERT INTO `alumno` VALUES (1,'Ana Pérez','2012-05-14 00:00:00',12,'Av. Central 123','5551234567','5557654321','Asma','Laura Pérez','laura.perez@mail.com',0),(2,'Luis Gómez','2010-09-22 00:00:00',14,'Calle Luna 45','5559876543','5553456789','Ninguno','Carlos Gómez','carlos.gomez@mail.com',1),(3,'María Torres','2011-12-01 00:00:00',13,'Calle Sol 88','5555678910','5556789012','Alergia a lácteos','Ana Torres','ana.torres@mail.com',0),(4,'Diego Sánchez','2013-07-03 00:00:00',11,'Av. Norte 77','5551122334','5554433221','TDAH','Juan Sánchez','juan.sanchez@mail.com',1),(5,'Laura Ruiz ','2009-03-17 00:00:00',15,'Calle Sur 12','5556677889','5559988776','Ninguno','Pedro Ruiz','pedro.ruiz@mail.com',0),(6,'Carlos Medina','2012-10-10 00:00:00',12,'Av. Este 9','5552233445','5555544332','Epilepsia','Marta Medina','marta.medina@mail.com',1),(7,'Sofía León','2011-06-28 00:00:00',13,'Calle Oeste 34','5553344556','5556655443','Asma','Ricardo León','ricardo.leon@mail.com',0),(8,'Mateo Ríos','2014-01-20 00:00:00',10,'Calle Palmera 4','5557788990','5558899001','Ninguno','Verónica Ríos','veronica.rios@mail.com',0),(9,'Valentina Sol','2010-11-11 00:00:00',14,'Av. Libertad 56','5559998887','5557776665','Autismo leve','Sergio Sol','sergio.sol@mail.com',1),(10,'Tomás Díaz','2013-04-05 00:00:00',11,'Calle Río 10','5553211234','5554312345','Ninguno','Gabriela Díaz','gabriela.diaz@mail.com',0),(11,'Emilia Torres','2012-09-15 00:00:00',13,'Calle Robles 123','5551001001','5552002002','Ninguno','Silvia Torres','silvia.torres@mail.com',0),(12,'Julián Ríos','2011-05-20 00:00:00',14,'Av. Central 45','5553003003','5554004004','Asma','Fernando Ríos','fernando.rios@mail.com',1),(13,'Isabela Cruz','2013-03-10 00:00:00',11,'Calle Luna 88','5555005005','5556006006','Ninguno','Laura Cruz','laura.cruz@mail.com',0),(14,'Benjamín Soto','2010-12-05 00:00:00',14,'Calle Sol 22','5557007007','5558008008','TDAH','Ricardo Soto','ricardo.soto@mail.com',1),(15,'Camila Vargas','2012-07-01 00:00:00',13,'Av. Norte 77','5559009009','5551112222','Alergia a maní','Marcela Vargas','marcela.vargas@mail.com',0),(16,'Daniel Herrera','2011-11-23 00:00:00',13,'Calle Sur 11','5553334444','5555556666','Ninguno','Javier Herrera','javier.herrera@mail.com',0),(17,'Valeria Luna','2010-08-08 00:00:00',15,'Av. Este 19','5557778888','5559990000','Autismo leve','Patricia Luna','patricia.luna@mail.com',1),(18,'Tomás Navarro','2013-10-10 00:00:00',11,'Calle Oeste 35','5551110000','5552221111','Epilepsia','Mario Navarro','mario.navarro@mail.com',1),(19,'Lucía Aguilar','2014-01-20 00:00:00',10,'Calle Palmera 14','5553332222','5554443333','Ninguno','Gabriela Aguilar','gabriela.aguilar@mail.com',0),(20,'Santiago Pérez','2012-04-30 00:00:00',13,'Av. Libertad 56','5555554444','5556665555','Asma','Rodrigo Pérez','rodrigo.perez@mail.com',1),(21,'Mía Morales','2011-02-14 00:00:00',14,'Calle Río 10','5557776666','5558887777','Ninguno','Andrea Morales','andrea.morales@mail.com',0),(22,'Leonardo Reyes','2013-06-06 00:00:00',11,'Calle Arce 7','5559998888','5550001111','Ninguno','Jorge Reyes','jorge.reyes@mail.com',0),(23,'Renata Fuentes','2010-09-09 00:00:00',15,'Av. Cedros 3','5551239876','5557896541','TDAH','Carla Fuentes','carla.fuentes@mail.com',1),(24,'Martín Carrillo','2011-12-12 00:00:00',13,'Calle Sauce 19','5554567890','5550987654','Alergia a polvo','Luis Carrillo','luis.carrillo@mail.com',0),(25,'Zoe Romero','2012-10-01 00:00:00',12,'Av. Magnolia 8','5555432109','5554321098','Ninguno','Elena Romero','elena.romero@mail.com',0),(26,'Iván Castillo','2014-05-05 00:00:00',10,'Calle Sauce 56','5553216549','5557893214','Asma','Sofía Castillo','sofia.castillo@mail.com',1),(27,'Ángela Mendoza','2013-08-18 00:00:00',11,'Av. Del Mar 44','5559638520','5558529631','Ninguno','Mariana Mendoza','mariana.mendoza@mail.com',0),(28,'Gabriel Silva','2011-01-02 00:00:00',14,'Calle Río Verde','5557418529','5551593572','TDAH','Daniela Silva','daniela.silva@mail.com',1),(29,'Paula Ortega','2010-07-13 00:00:00',15,'Av. del Bosque','5553571598','5559517532','Ninguno','Camilo Ortega','camilo.ortega@mail.com',0),(30,'Andrés Pineda','2012-11-29 00:00:00',12,'Calle Roble 10','5552581470','5551472583','Alergia a gluten','Lucía Pineda','lucia.pineda@mail.com',1),(31,'Ana Pérez','2012-05-14 00:00:00',12,'Av. Central 123','5551234567','5557654321','Asma','Laura Pérez','laura.perez@mail.com',0),(32,'Luis Gómez','2010-09-22 00:00:00',14,'Calle Luna 45','5559876543','5553456789','Ninguno','Carlos Gómez','carlos.gomez@mail.com',1),(33,'María Torres','2011-12-01 00:00:00',13,'Calle Sol 88','5555678910','5556789012','Alergia a lácteos','Ana Torres','ana.torres@mail.com',0),(34,'Diego Sánchez','2013-07-03 00:00:00',11,'Av. Norte 77','5551122334','5554433221','TDAH','Juan Sánchez','juan.sanchez@mail.com',1),(35,'Laura Ruiz','2009-03-17 00:00:00',15,'Calle Sur 12','5556677889','5559988776','Ninguno','Pedro Ruiz','pedro.ruiz@mail.com',0),(36,'Carlos Medina','2012-10-10 00:00:00',12,'Av. Este 9','5552233445','5555544332','Epilepsia','Marta Medina','marta.medina@mail.com',1),(37,'Sofía León','2011-06-28 00:00:00',13,'Calle Oeste 34','5553344556','5556655443','Asma','Ricardo León','ricardo.leon@mail.com',0),(38,'Mateo Ríos','2014-01-20 00:00:00',10,'Calle Palmera 4','5557788990','5558899001','Ninguno','Verónica Ríos','veronica.rios@mail.com',0),(39,'Valentina Sol','2010-11-11 00:00:00',14,'Av. Libertad 56','5559998887','5557776665','Autismo leve','Sergio Sol','sergio.sol@mail.com',1),(40,'Tomás Díaz','2013-04-05 00:00:00',11,'Calle Río 10','5553211234','5554312345','Ninguno','Gabriela Díaz','gabriela.diaz@mail.com',0),(41,'Emilia Torres','2012-09-15 00:00:00',13,'Calle Robles 123','5551001001','5552002002','Ninguno','Silvia Torres','silvia.torres@mail.com',0),(42,'Julián Ríos','2011-05-20 00:00:00',14,'Av. Central 45','5553003003','5554004004','Asma','Fernando Ríos','fernando.rios@mail.com',1),(43,'Isabela Cruz','2013-03-10 00:00:00',11,'Calle Luna 88','5555005005','5556006006','Ninguno','Laura Cruz','laura.cruz@mail.com',0),(44,'Benjamín Soto','2010-12-05 00:00:00',14,'Calle Sol 22','5557007007','5558008008','TDAH','Ricardo Soto','ricardo.soto@mail.com',1),(45,'Camila Vargas','2012-07-01 00:00:00',13,'Av. Norte 77','5559009009','5551112222','Alergia a maní','Marcela Vargas','marcela.vargas@mail.com',0),(46,'Daniel Herrera','2011-11-23 00:00:00',13,'Calle Sur 11','5553334444','5555556666','Ninguno','Javier Herrera','javier.herrera@mail.com',0),(47,'Valeria Luna','2010-08-08 00:00:00',15,'Av. Este 19','5557778888','5559990000','Autismo leve','Patricia Luna','patricia.luna@mail.com',1),(48,'Tomás Navarro','2013-10-10 00:00:00',11,'Calle Oeste 35','5551110000','5552221111','Epilepsia','Mario Navarro','mario.navarro@mail.com',1),(49,'Lucía Aguilar','2014-01-20 00:00:00',10,'Calle Palmera 14','5553332222','5554443333','Ninguno','Gabriela Aguilar','gabriela.aguilar@mail.com',0),(50,'Santiago Pérez','2012-04-30 00:00:00',13,'Av. Libertad 56','5555554444','5556665555','Asma','Rodrigo Pérez','rodrigo.perez@mail.com',1),(51,'Mía Morales','2011-02-14 00:00:00',14,'Calle Río 10','5557776666','5558887777','Ninguno','Andrea Morales','andrea.morales@mail.com',0),(52,'Leonardo Reyes','2013-06-06 00:00:00',11,'Calle Arce 7','5559998888','5550001111','Ninguno','Jorge Reyes','jorge.reyes@mail.com',0),(53,'Renata Fuentes','2010-09-09 00:00:00',15,'Av. Cedros 3','5551239876','5557896541','TDAH','Carla Fuentes','carla.fuentes@mail.com',1),(54,'Martín Carrillo','2011-12-12 00:00:00',13,'Calle Sauce 19','5554567890','5550987654','Alergia a polvo','Luis Carrillo','luis.carrillo@mail.com',0),(55,'Zoe Romero','2012-10-01 00:00:00',12,'Av. Magnolia 8','5555432109','5554321098','Ninguno','Elena Romero','elena.romero@mail.com',0),(56,'Iván Castillo','2014-05-05 00:00:00',10,'Calle Sauce 56','5553216549','5557893214','Asma','Sofía Castillo','sofia.castillo@mail.com',1),(57,'Ángela Mendoza','2013-08-18 00:00:00',11,'Av. Del Mar 44','5559638520','5558529631','Ninguno','Mariana Mendoza','mariana.mendoza@mail.com',0),(58,'Gabriel Silva','2011-01-02 00:00:00',14,'Calle Río Verde','5557418529','5551593572','TDAH','Daniela Silva','daniela.silva@mail.com',1),(59,'Paula Ortega','2010-07-13 00:00:00',15,'Av. del Bosque','5553571598','5559517532','Ninguno','Camilo Ortega','camilo.ortega@mail.com',0),(60,'Andrés Pineda','2012-11-29 00:00:00',12,'Calle Roble 10','5552581470','5551472583','Alergia a gluten','Lucía Pineda','lucia.pineda@mail.com',1),(61,'Ana Pérez','2012-05-14 00:00:00',12,'Av. Central 123','5551234567','5557654321','Asma','Laura Pérez','laura.perez@mail.com',0),(62,'Luis Gómez','2010-09-22 00:00:00',14,'Calle Luna 45','5559876543','5553456789','Ninguno','Carlos Gómez','carlos.gomez@mail.com',1),(63,'María Torres','2011-12-01 00:00:00',13,'Calle Sol 88','5555678910','5556789012','Alergia a lácteos','Ana Torres','ana.torres@mail.com',0),(64,'Diego Sánchez','2013-07-03 00:00:00',11,'Av. Norte 77','5551122334','5554433221','TDAH','Juan Sánchez','juan.sanchez@mail.com',1),(65,'Laura Ruiz','2009-03-17 00:00:00',15,'Calle Sur 12','5556677889','5559988776','Ninguno','Pedro Ruiz','pedro.ruiz@mail.com',0),(66,'Carlos Medina','2012-10-10 00:00:00',12,'Av. Este 9','5552233445','5555544332','Epilepsia','Marta Medina','marta.medina@mail.com',1),(67,'Sofía León','2011-06-28 00:00:00',13,'Calle Oeste 34','5553344556','5556655443','Asma','Ricardo León','ricardo.leon@mail.com',0),(68,'Mateo Ríos','2014-01-20 00:00:00',10,'Calle Palmera 4','5557788990','5558899001','Ninguno','Verónica Ríos','veronica.rios@mail.com',0),(69,'Valentina Sol','2010-11-11 00:00:00',14,'Av. Libertad 56','5559998887','5557776665','Autismo leve','Sergio Sol','sergio.sol@mail.com',1),(70,'Tomás Díaz','2013-04-05 00:00:00',11,'Calle Río 10','5553211234','5554312345','Ninguno','Gabriela Díaz','gabriela.diaz@mail.com',0),(71,'Emilia Torres','2012-09-15 00:00:00',13,'Calle Robles 123','5551001001','5552002002','Ninguno','Silvia Torres','silvia.torres@mail.com',0),(72,'Julián Ríos','2011-05-20 00:00:00',14,'Av. Central 45','5553003003','5554004004','Asma','Fernando Ríos','fernando.rios@mail.com',1),(73,'Isabela Cruz','2013-03-10 00:00:00',11,'Calle Luna 88','5555005005','5556006006','Ninguno','Laura Cruz','laura.cruz@mail.com',0),(74,'Benjamín Soto','2010-12-05 00:00:00',14,'Calle Sol 22','5557007007','5558008008','TDAH','Ricardo Soto','ricardo.soto@mail.com',1),(75,'Camila Vargas','2012-07-01 00:00:00',13,'Av. Norte 77','5559009009','5551112222','Alergia a maní','Marcela Vargas','marcela.vargas@mail.com',0),(76,'Daniel Herrera','2011-11-23 00:00:00',13,'Calle Sur 11','5553334444','5555556666','Ninguno','Javier Herrera','javier.herrera@mail.com',0),(77,'Valeria Luna','2010-08-08 00:00:00',15,'Av. Este 19','5557778888','5559990000','Autismo leve','Patricia Luna','patricia.luna@mail.com',1),(78,'Tomás Navarro','2013-10-10 00:00:00',11,'Calle Oeste 35','5551110000','5552221111','Epilepsia','Mario Navarro','mario.navarro@mail.com',1),(79,'Lucía Aguilar','2014-01-20 00:00:00',10,'Calle Palmera 14','5553332222','5554443333','Ninguno','Gabriela Aguilar','gabriela.aguilar@mail.com',0),(80,'Santiago Pérez','2012-04-30 00:00:00',13,'Av. Libertad 56','5555554444','5556665555','Asma','Rodrigo Pérez','rodrigo.perez@mail.com',1),(81,'Mía Morales','2011-02-14 00:00:00',14,'Calle Río 10','5557776666','5558887777','Ninguno','Andrea Morales','andrea.morales@mail.com',0),(82,'Leonardo Reyes','2013-06-06 00:00:00',11,'Calle Arce 7','5559998888','5550001111','Ninguno','Jorge Reyes','jorge.reyes@mail.com',0),(83,'Renata Fuentes','2010-09-09 00:00:00',15,'Av. Cedros 3','5551239876','5557896541','TDAH','Carla Fuentes','carla.fuentes@mail.com',1),(84,'Martín Carrillo','2011-12-12 00:00:00',13,'Calle Sauce 19','5554567890','5550987654','Alergia a polvo','Luis Carrillo','luis.carrillo@mail.com',0),(85,'Zoe Romero','2012-10-01 00:00:00',12,'Av. Magnolia 8','5555432109','5554321098','Ninguno','Elena Romero','elena.romero@mail.com',0),(86,'Iván Castillo','2014-05-05 00:00:00',10,'Calle Sauce 56','5553216549','5557893214','Asma','Sofía Castillo','sofia.castillo@mail.com',1),(87,'Ángela Mendoza','2013-08-18 00:00:00',11,'Av. Del Mar 44','5559638520','5558529631','Ninguno','Mariana Mendoza','mariana.mendoza@mail.com',0),(88,'Gabriel Silva','2011-01-02 00:00:00',14,'Calle Río Verde','5557418529','5551593572','TDAH','Daniela Silva','daniela.silva@mail.com',1),(89,'Paula Ortega','2010-07-13 00:00:00',15,'Av. del Bosque','5553571598','5559517532','Ninguno','Camilo Ortega','camilo.ortega@mail.com',0),(90,'Andrés Pineda','2012-11-29 00:00:00',12,'Calle Roble 10','5552581470','5551472583','Alergia a gluten','Lucía Pineda','lucia.pineda@mail.com',1),(91,'Andrés Pinedav','2012-11-29 00:00:00',12,'Calle Roble 10','5552581470','5551472583','Alergia a gluten','Lucía Pineda','lucia.pineda@mail.com',1);
/*!40000 ALTER TABLE `alumno` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `listatalleres`
--

DROP TABLE IF EXISTS `listatalleres`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `listatalleres` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `IdAlumno` int NOT NULL,
  `IdTaller` int NOT NULL,
  `FechaRegistro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fechaCita` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fkListaAlumno_idx` (`IdAlumno`),
  KEY `fkListaTaller_idx` (`IdTaller`),
  CONSTRAINT `fkListaAlumno` FOREIGN KEY (`IdAlumno`) REFERENCES `alumno` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fkListaTaller` FOREIGN KEY (`IdTaller`) REFERENCES `taller` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `listatalleres`
--

LOCK TABLES `listatalleres` WRITE;
/*!40000 ALTER TABLE `listatalleres` DISABLE KEYS */;
INSERT INTO `listatalleres` VALUES (1,1,2,'2025-09-25 12:54:54','lunes'),(2,2,47,'2025-09-25 12:56:04',NULL),(5,2,48,'2025-10-07 14:09:48',NULL);
/*!40000 ALTER TABLE `listatalleres` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol`
--

DROP TABLE IF EXISTS `rol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Rol` varchar(15) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol`
--

LOCK TABLES `rol` WRITE;
/*!40000 ALTER TABLE `rol` DISABLE KEYS */;
INSERT INTO `rol` VALUES (1,'Administrador'),(2,'Profesor'),(3,'Visitante');
/*!40000 ALTER TABLE `rol` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taller`
--

DROP TABLE IF EXISTS `taller`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `taller` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(30) NOT NULL,
  `Lugares_Disp` int NOT NULL,
  `Dias` varchar(40) NOT NULL,
  `conCita` tinyint NOT NULL DEFAULT '0',
  `Hora_inicio` time NOT NULL,
  `Hora_final` time NOT NULL,
  `Edad_min` int NOT NULL,
  `Edad_max` int DEFAULT NULL,
  `Estado` tinyint NOT NULL,
  `Costo` decimal(10,2) NOT NULL,
  `IdUsuario` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fkTallerUsuario_idx` (`IdUsuario`),
  CONSTRAINT `fkTallerUsuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=76 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taller`
--

LOCK TABLES `taller` WRITE;
/*!40000 ALTER TABLE `taller` DISABLE KEYS */;
INSERT INTO `taller` VALUES (1,'Ingles general',10,'lunes, jueves',0,'07:00:00','08:00:00',18,NULL,1,200.00,4),(2,'YogaKids',10,'lunes, miercoles, viernes',0,'15:30:00','16:30:00',7,12,1,200.00,5),(3,'PintArte',15,'viernes',0,'15:00:00','17:00:00',7,11,1,200.00,2),(4,'Pinturas jovenes',10,'sabado',0,'10:30:00','12:00:00',12,NULL,1,200.00,2),(5,'Coro CCLEMS',15,'lunes, miercoles',0,'18:00:00','19:00:00',10,NULL,1,200.00,4),(6,'Ingles infantil B',10,'martes, jueves',0,'16:30:00','15:30:00',8,11,1,200.00,4),(7,'Ingles jovenes',12,'martes, jueves',0,'18:00:00','19:00:00',12,17,1,200.00,4),(8,'Lectura de notas musicales',12,'viernes',0,'18:00:00','19:00:00',12,NULL,1,200.00,4),(9,'Piano sabatino',10,'sabado',0,'11:30:00','12:30:00',12,NULL,1,200.00,4),(10,'Baile moderno',12,'lunes, miercoles, viernes',0,'16:30:00','17:30:00',7,11,1,200.00,5),(11,'MateTutor',10,'martes, jueves',0,'17:00:00','18:00:00',7,10,1,200.00,5),(12,'Tertulia literaria',10,'viernes',0,'18:00:00','19:00:00',18,NULL,1,200.00,5),(13,'Jugando con ciencia',12,'sabado',0,'12:00:00','13:00:00',8,10,1,200.00,5),(14,'Lectoescritura',10,'martes, jueves',0,'16:00:00','17:00:00',7,10,1,200.00,5),(15,'Piano general',12,'lunes, miercoles, viernes',0,'17:00:00','18:00:00',12,NULL,1,200.00,4),(16,'Piano infantil B',15,'lunes, miercoles, viernes',0,'16:00:00','17:00:00',8,11,1,200.00,4),(17,'Piano infantil A',15,'lunes, miercoles, viernes',0,'15:00:00','16:00:00',8,11,1,200.00,4),(18,'Ingles infantil A',10,'martes, jueves',0,'15:00:00','16:00:00',8,11,1,200.00,4),(19,'Ingles sabatino',10,'sabado',0,'10:00:00','11:30:00',12,NULL,1,200.00,4),(20,'Atencion psicopedagogica',3,'cita',1,'14:00:00','17:00:00',4,NULL,1,200.00,6),(21,'Guitarra infantil',15,'lunes, miercoles, viernes',0,'18:00:00','19:00:00',8,12,1,200.00,6),(22,'Guitarra publico en general',15,'martes, jueves',0,'18:00:00','19:00:00',13,NULL,1,200.00,6),(23,'Teatro',12,'lunes, miercoles',0,'16:00:00','17:00:00',8,12,1,200.00,6),(24,'Terapia de lenguaje',5,'miercoles, viernes',0,'15:00:00','16:00:00',4,6,1,200.00,6),(25,'Violin',15,'martes, miercoles, jueves',0,'17:00:00','18:00:00',13,NULL,1,200.00,6),(26,'Violin infantil',12,'martes, miercoles, jueves',0,'19:00:00','20:00:00',8,12,1,200.00,6),(27,'Ajedrez infantil',12,'martes, jueves',0,'15:00:00','16:00:00',7,11,1,200.00,7),(28,'Ajedrez',12,'martes, jueves',0,'17:00:00','18:00:00',12,NULL,1,200.00,7),(29,'Ajedrez sabatino',12,'sabado',0,'14:00:00','15:00:00',40,NULL,1,200.00,7),(30,'Computacion con DataCamp',12,'martes, jueves',0,'18:00:00','19:00:00',16,NULL,1,200.00,7),(31,'Computacion infantil A',12,'lunes, miercoles',0,'15:00:00','16:00:00',7,11,1,200.00,7),(32,'Computacion infantil B',12,'miercoles, viernes',0,'16:00:00','17:00:00',7,11,1,200.00,7),(33,'Excel',15,'lunes, miercoles',0,'18:00:00','19:00:00',17,NULL,1,200.00,7),(34,'Juego de tarjetas',10,'sabado',0,'14:00:00','17:00:00',13,NULL,1,200.00,7),(35,'Dibujo a pastel',15,'lunes, miercoles',0,'17:00:00','18:00:00',7,11,1,200.00,8),(36,'AcuarelaKids B',12,'viernes',0,'04:00:00','05:00:00',4,6,1,200.00,8),(37,'Manualidades',12,'martes, jueves',0,'16:00:00','17:00:00',4,6,1,200.00,8),(38,'MoldeArte jovenes',10,'viernes',0,'17:00:00','18:00:00',12,NULL,1,200.00,8),(39,'MoldeArte Kids',12,'martes, jueves',0,'17:00:00','18:00:00',7,11,1,200.00,8),(40,'PintaKids A',15,'lunes, miercoles, viernes',0,'15:00:00','16:00:00',7,11,1,200.00,8),(41,'PintaKids B',15,'martes, jueves',0,'15:00:00','16:00:00',7,11,1,200.00,8),(42,'Aprender jugando',15,'lunes, miercoles',0,'16:30:00','17:30:00',4,6,1,200.00,2),(43,'Baile para peques',15,'lunes, miercoles',0,'15:00:00','16:00:00',4,6,1,200.00,2),(44,'PintArte',14,'viernes',0,'15:00:00','17:00:00',7,11,1,200.00,2),(45,'Pintura jovenes',14,'sabado',0,'10:30:00','12:00:00',12,NULL,1,200.00,2),(46,'Taller de titeres',12,'martes, jueves',0,'16:30:00','17:30:00',9,12,1,200.00,2),(47,'Dibujo digital',12,'miercoles, viernes',0,'17:00:00','18:00:00',7,11,1,200.00,9),(48,'Fotografia',13,'martes, jueves',0,'17:00:00','18:00:00',15,NULL,1,200.00,9),(49,'Folclor infantil',15,'lunes, miercoles, jueves',0,'17:45:00','18:45:00',8,11,1,200.00,10),(72,'traaasss',1,'martes',0,'00:00:10','00:00:11',2,8,1,200.00,2),(73,'hola',0,'lunes',0,'10:00:00','11:00:00',4,6,1,200.00,2),(74,'buu',0,'martes',0,'00:00:10','00:00:11',4,6,0,200.00,2),(75,'traass',0,'jueves',0,'10:00:00','11:00:00',5,8,1,200.00,5);
/*!40000 ALTER TABLE `taller` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(45) NOT NULL,
  `Correo` varchar(50) NOT NULL,
  `numTel` varchar(15) NOT NULL,
  `contraseña` varchar(45) NOT NULL,
  `IdRol` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fkUsuarioRol_idx` (`IdRol`),
  CONSTRAINT `fkUsuarioRol` FOREIGN KEY (`IdRol`) REFERENCES `rol` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'Isac Zapata Moreno','isac.zapata@centroculturallems.com','8616121225','123456789',1),(2,'Selena García Velázquez','selena.garcia@centroculturallems.com','8616121225','123456789',2),(3,'Yolanda Gabriela Gallegos Valdes','yolanda.gallegos@centroculturallems.com','8616121225','123456789',1),(4,'Luis José Garza Valdez','luis.garza@centroculturallems.com','8616121225','123456789',2),(5,'Carolina Guajardo García','carolina.guajardo@centroculturallems.com','8616121225','123456789',2),(6,'Pablo Márquez','pablo.marquez@centroculturallems.com','8616121225','123456789',2),(7,'César Gaytan','cesar.gaytan@centroculturallems.com','8616121225','123456789',2),(8,'Ángeles Castillo','angeles.castillo@centroculturallems.com','8616121225','123456789',2),(9,'David Infante','david.infante@centroculturallems.com','8616121225','123456789',2),(10,'Barbara Valenzuela','barbara.valen@centroculturallems.com','8616121225','123456789',2);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-10-14 10:49:25
