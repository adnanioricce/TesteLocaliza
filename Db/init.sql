/*

**/
CREATE TABLE Usuarios(
    Id SERIAL
    ,Nome VARCHAR(64)
    ,Email VARCHAR(64)
    ,HashSenha VARCHAR(512)
    ,PRIMARY KEY (Id)
);

CREATE TABLE Clientes(
    Id SERIAL
    ,UsuarioId INTEGER REFERENCES Usuarios (Id)
    ,Documento VARCHAR(16)
    ,Nome VARCHAR(64)    
    ,Telefone VARCHAR(512)
    ,Endereco VARCHAR(64)
    ,PRIMARY KEY(Id)
);

CREATE TABLE Cobrancas(
    Id SERIAL
    ,ClienteId INTEGER REFERENCES Clientes (Id)
    ,Valor DECIMAL(10,2)
    ,DataVencimento DATE
    ,Pago BOOLEAN DEFAULT FALSE
    ,Descricao VARCHAR(512)
    ,PRIMARY KEY(Id)
);
