-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: proyectoregistro
-- ------------------------------------------------------
-- Server version	8.0.43

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
  `IdUsuario` int DEFAULT NULL,
  `Inscritos` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `fkTallerUsuario_idx` (`IdUsuario`),
  CONSTRAINT `fkTallerUsuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=99 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taller`
--

LOCK TABLES `taller` WRITE;
/*!40000 ALTER TABLE `taller` DISABLE KEYS */;
INSERT INTO `taller` VALUES (1,'Ingles general',10,'lunes, jueves',0,'07:00:00','08:00:00',18,NULL,1,200.00,4,0),(2,'YogaKids',10,'lunes, miercoles, viernes',0,'15:30:00','16:30:00',7,12,1,200.00,5,0),(3,'PintArte',16,'viernes',0,'15:00:00','17:00:00',7,11,1,200.00,NULL,0),(4,'Pinturas jovenes',10,'sabado',0,'10:30:00','12:00:00',12,NULL,1,200.00,NULL,0),(5,'Coro CCLEMS',16,'lunes, miercoles',0,'18:00:00','19:00:00',10,NULL,1,200.00,4,0),(6,'Ingles infantil B',10,'martes, jueves',0,'16:30:00','15:30:00',8,11,1,200.00,4,0),(7,'Ingles jovenes',12,'martes, jueves',0,'18:00:00','19:00:00',12,17,1,200.00,4,0),(8,'Lectura de notas musicales',12,'viernes',0,'18:00:00','19:00:00',12,NULL,1,200.00,4,0),(9,'Piano sabatino',10,'sabado',0,'11:30:00','12:30:00',12,NULL,1,200.00,4,0),(10,'Baile moderno',12,'lunes, miercoles, viernes',0,'16:30:00','17:30:00',7,11,1,200.00,5,0),(11,'MateTutor',10,'martes, jueves',0,'17:00:00','18:00:00',7,10,1,200.00,5,0),(12,'Tertulia literaria',10,'viernes',0,'18:00:00','19:00:00',18,NULL,1,200.00,5,0),(13,'Jugando con ciencia',12,'sabado',0,'12:00:00','13:00:00',8,10,1,200.00,5,0),(14,'Lectoescritura',10,'martes, jueves',0,'16:00:00','17:00:00',7,10,1,200.00,5,0),(15,'Piano general',12,'lunes, miercoles, viernes',0,'17:00:00','18:00:00',12,NULL,1,200.00,4,0),(16,'Piano infantil B',15,'lunes, miercoles, viernes',0,'16:00:00','17:00:00',8,11,1,200.00,4,0),(17,'Piano infantil A',15,'lunes, miercoles, viernes',0,'15:00:00','16:00:00',8,11,1,200.00,4,0),(18,'Ingles infantil A',10,'martes, jueves',0,'15:00:00','16:00:00',8,11,1,200.00,4,0),(19,'Ingles sabatino',10,'sabado',0,'10:00:00','11:30:00',12,NULL,1,200.00,4,0),(20,'Atencion psicopedagogica',3,'cita',1,'14:00:00','17:00:00',4,NULL,1,200.00,6,0),(21,'Guitarra infantil',15,'lunes, miercoles, viernes',0,'18:00:00','19:00:00',8,12,1,200.00,6,0),(22,'Guitarra publico en general',15,'martes, jueves',0,'18:00:00','19:00:00',13,NULL,1,200.00,6,0),(23,'Teatro',12,'lunes, miercoles',0,'16:00:00','17:00:00',8,12,1,200.00,6,0),(24,'Terapia de lenguaje',5,'miercoles, viernes',0,'15:00:00','16:00:00',4,6,1,200.00,6,0),(25,'Violin',15,'martes, miercoles, jueves',0,'17:00:00','18:00:00',13,NULL,1,200.00,6,0),(26,'Violin infantil',12,'martes, miercoles, jueves',0,'19:00:00','20:00:00',8,12,1,200.00,6,0),(27,'Ajedrez infantillll',12,'martes, jueves',0,'03:00:00','04:00:00',9,11,1,200.00,7,0),(28,'Ajedrezz',12,'martes, jueves',0,'05:00:00','06:00:00',12,NULL,0,200.00,7,0),(29,'Ajedrez sabatino',12,'sabado',0,'14:00:00','15:00:00',40,NULL,1,200.00,7,0),(30,'Computacion con DataCamp',12,'martes, jueves',0,'18:00:00','19:00:00',16,NULL,1,200.00,7,0),(31,'Computacion infantil A',12,'lunes, miercoles',0,'15:00:00','16:00:00',7,11,1,200.00,7,0),(32,'Computacion infantil B',12,'miercoles, viernes',0,'16:00:00','17:00:00',7,11,1,200.00,7,0),(33,'Excel',15,'lunes, miercoles',0,'18:00:00','19:00:00',17,NULL,1,200.00,7,0),(34,'Juego de tarjetas',10,'sabado',0,'14:00:00','17:00:00',13,NULL,1,200.00,7,0),(35,'Dibujo a pastel',15,'lunes, miercoles',0,'17:00:00','18:00:00',7,11,1,200.00,8,0),(36,'AcuarelaKids B',12,'viernes',0,'04:00:00','05:00:00',4,6,1,200.00,8,0),(37,'Manualidades',12,'martes, jueves',0,'16:00:00','17:00:00',4,6,1,200.00,8,0),(38,'MoldeArte jovenes',10,'viernes',0,'17:00:00','18:00:00',12,NULL,1,200.00,8,0),(39,'MoldeArte Kids',12,'martes, jueves',0,'17:00:00','18:00:00',7,11,1,200.00,8,0),(40,'PintaKids A',15,'lunes, miercoles, viernes',0,'15:00:00','16:00:00',7,11,1,200.00,8,0),(41,'PintaKids B',15,'martes, jueves',0,'15:00:00','16:00:00',7,11,1,200.00,8,0),(42,'Aprender jugando',15,'lunes, miercoles',0,'16:30:00','17:30:00',4,6,0,200.00,NULL,0),(43,'Baile para peques',15,'lunes, miercoles',0,'15:00:00','16:00:00',4,6,0,200.00,NULL,0),(44,'PintArte',14,'viernes',0,'15:00:00','17:00:00',7,11,0,200.00,NULL,0),(45,'Pintura jovenes',14,'sabado',0,'10:30:00','12:00:00',12,NULL,1,200.00,8,0),(46,'Taller de titeres',12,'martes, jueves',0,'16:30:00','17:30:00',9,12,0,200.00,NULL,0),(47,'Dibujo digital',12,'miercoles, viernes',0,'17:00:00','18:00:00',7,11,1,200.00,9,0),(48,'Fotografia',13,'martes, jueves',0,'17:00:00','18:00:00',15,NULL,1,200.00,9,0),(49,'Folclor infantil',15,'lunes, miercoles, jueves',0,'17:45:00','18:45:00',8,11,1,200.00,10,0),(72,'traaasss',1,'martes',0,'00:00:10','00:00:11',2,8,0,200.00,NULL,0),(73,'hola',0,'lunes',0,'10:00:00','11:00:00',4,6,1,200.00,7,0),(74,'buu',0,'martes',0,'00:00:10','00:00:11',4,6,0,200.00,NULL,0),(75,'traass',0,'jueves',0,'10:00:00','11:00:00',5,8,0,200.00,5,0),(78,'prueba1',20,'miércoles',0,'01:02:00','02:02:00',5,NULL,0,200.00,7,0),(79,'aaa',5,'d',0,'01:04:00','03:04:00',5,8,0,30.00,5,0),(81,'aprueba1',36,'miércoles',0,'03:45:00','04:45:00',1,3,0,200.00,5,0),(82,'mmm0000',28,'juevevesmmm',0,'01:26:00','01:26:00',25,30,0,200.00,8,0),(83,'prueba1',30,'martes, jueves',0,'14:14:00','15:14:00',12,16,0,400.00,4,0),(84,'nose',4,'martes, jueves',0,'02:15:00','03:15:00',25,NULL,0,233.00,NULL,0),(88,'yoppp',30,'martes, jueves',0,'04:38:00','06:38:00',3,5,0,200.00,NULL,0),(90,'aaaapa',20,'sabado',0,'14:00:00','15:00:00',10,NULL,0,20.00,NULL,0),(91,'aaaaaaaaaaaaaaaaaaaaaa',10,'miércoles',0,'16:00:00','05:00:00',3,NULL,0,20.00,NULL,0),(92,'asumecha',10,'martes, jueves',0,'04:00:00','05:00:00',20,NULL,0,20.00,NULL,0),(93,'asap',20,'miércoles',0,'14:00:00','15:00:00',13,NULL,0,20.00,NULL,0),(94,'asuususu',20,'miércoles',0,'01:04:00','03:04:00',3,NULL,0,200.00,NULL,0),(95,'trakis',20,'martes, jueves',0,'11:00:00','00:00:00',12,NULL,0,20.00,NULL,0),(96,'mAUUS',20,'martes, jueves',0,'10:30:00','11:31:00',5,NULL,0,0.00,NULL,0),(97,'aprobado',20,'miercoles',0,'23:55:00','23:55:00',1,2,1,1.00,2,0),(98,'Aprobado',20,'mier',0,'11:07:00','11:27:00',1,2,1,200.00,4,0);
/*!40000 ALTER TABLE `taller` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-25 11:41:48
