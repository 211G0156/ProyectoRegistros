CREATE TABLE ListaEspera (
    Id INT AUTO_INCREMENT PRIMARY KEY,         
    IdAlumno INT NOT NULL,
    IdTaller INT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,  
    Estado VARCHAR(20) NOT NULL DEFAULT 'En espera',

    CONSTRAINT FK_ListaEspera_Alumno FOREIGN KEY (IdAlumno) REFERENCES Alumno(Id),
    CONSTRAINT FK_ListaEspera_Taller FOREIGN KEY (IdTaller) REFERENCES Taller(Id)
);
