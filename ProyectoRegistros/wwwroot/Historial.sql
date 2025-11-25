CREATE TABLE `historial` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `idUsuario` int NOT NULL,
  `idAlumno` int NOT NULL,
  `idTaller` int NOT NULL,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `mensaje` text NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fkHistorialUsuario` (`idUsuario`),
  KEY `fkHistorialAlumno` (`idAlumno`),
  KEY `fkHistorialTaller` (`idTaller`),
  CONSTRAINT `fkHistorialAlumno` FOREIGN KEY (`idAlumno`) REFERENCES `alumno` (`Id`),
  CONSTRAINT `fkHistorialTaller` FOREIGN KEY (`idTaller`) REFERENCES `taller` (`Id`),
  CONSTRAINT `fkHistorialUsuario` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`Id`)
) 