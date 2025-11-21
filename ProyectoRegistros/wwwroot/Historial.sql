
CREATE TABLE `historial` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `idUsuario` int NOT NULL,
  `idTaller` int NOT NULL,
  `idAlumno` int NOT NULL,
  fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `fkHistorialUsuario_idx` (`idUsuario`),
  KEY `fkHistorialTaller_idx` (`idTaller`),
  KEY `fkHistorialAlumno_idx` (`idAlumno`),
  CONSTRAINT `fkHistorialAlumno` FOREIGN KEY (`idAlumno`) REFERENCES `alumno` (`Id`),
  CONSTRAINT `fkHistorialTaller` FOREIGN KEY (`idTaller`) REFERENCES `taller` (`Id`),
  CONSTRAINT `fkHistorialUsuario` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`Id`)
)