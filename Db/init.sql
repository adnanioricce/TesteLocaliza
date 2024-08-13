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

INSERT INTO usuarios (nome,email,hashsenha) VALUES
 ('Annette Hermiston','Adolphus_Bartoletti@yahoo.com','jOBhMvPxAjUQS0mR9rGly/Bd4m6emGxHXQeTvNJx1s8='),
 ('Krista Prosacco','Jermain_Beer94@gmail.com','BZVZK+WufZ4mTvGGQ85xEgxYx0cWO/eIUsqrpwHkVbo='),
 ('Ashley Hoppe','Brennan.Stark20@gmail.com','Oe8QlEVNZlrbOOpmh7GKDNGOq4lPdCR9M/LRmucAd7E=');
	
INSERT INTO clientes (usuarioid,documento,nome,telefone,endereco) VALUES
 (2,'226.966.483-36','Kristin Kuhn','1-295-939-7712','67177 Jacobson Fork, Albertaview, Central African Republic'),
 (1,'607.157.806-03','Yolanda Upton','1-563-883-8277 x6360','35760 McGlynn Ridges, East Krystel, Mauritania'),
 (3,'608.175.600-92','Yvonne Fahey','1-654-373-4516 x566','15074 Hayes Curve, Lake Elyssaberg, Cayman Islands'),
 (3,'701.214.888-88','Sarah Kerluke','(236) 618-7994','7833 Cayla Turnpike, Port Audrey, Zimbabwe'),
 (1,'002.142.872-70','Kenneth Blick','(609) 411-4401 x283','358 Welch Pike, West Cyrus, Iceland'),
 (3,'771.726.963-52','Jonathan Kiehn','677-899-7339 x4729','71879 Frederik Place, Lake Romaine, Brazil'),
 (1,'516.479.713-57','Chelsea Oberbrunner','(332) 712-0892 x7519','94549 Fae Cliffs, West Gwendolynborough, Haiti'),
 (1,'591.138.120-68','teste','11441899234','Rua João Gomes');

INSERT INTO cobrancas (clienteid,valor,datavencimento,pago,descricao) VALUES
 (2,558.90,'2025-04-27',true,'Autem voluptatem natus nulla cum.'),
 (2,268.09,'2025-07-21',true,'Quibusdam rerum laborum exercitationem fuga.'),
 (2,399.23,'2024-10-04',false,'Cum nesciunt delectus incidunt sint est.'),
 (2,142.08,'2024-10-23',true,'Occaecati deserunt tenetur magnam.'),
 (2,264.21,'2025-03-16',true,'Ducimus velit vero sit illo dolor.'),
 (2,601.61,'2025-04-26',true,'Facilis deleniti consequatur ea provident nisi reprehenderit quos nihil quia.'),
 (2,715.15,'2024-10-24',true,'Facilis similique voluptatem.'),
 (2,661.08,'2025-04-12',true,'Rerum inventore est dolor et rerum.'),
 (2,227.92,'2025-05-09',false,'Distinctio et consequatur ut vero quo expedita.'),
 (2,744.49,'2024-11-10',true,'Quas animi cum doloribus itaque qui.');

INSERT INTO cobrancas (clienteid,valor,datavencimento,pago,descricao) VALUES
 (4,949.88,'2024-12-29',true,'Veniam et at iste inventore voluptas debitis.'),
 (4,876.98,'2025-02-02',false,'Ducimus eius non recusandae aut.'),
 (4,498.08,'2024-10-05',true,'Maiores distinctio laborum distinctio minima.'),
 (4,741.17,'2024-10-01',false,'Voluptatem eius animi.'),
 (4,639.44,'2025-06-28',true,'Quae excepturi molestias ipsum dolores minima autem fugiat et soluta.'),
 (4,275.02,'2024-12-08',false,'Hic et ipsa at nulla ipsa ullam.'),
 (4,309.96,'2024-12-02',true,'Incidunt cum id velit perspiciatis ab voluptas reprehenderit quaerat doloremque.'),
 (4,167.68,'2025-03-12',true,'Sint incidunt beatae in vel.'),
 (4,591.03,'2025-01-31',true,'Est est aut quis eveniet.'),
 (4,988.92,'2024-09-05',true,'Aut suscipit maxime odio quia culpa.');
	
INSERT INTO cobrancas (clienteid,valor,datavencimento,pago,descricao) VALUES
 (6,812.19,'2025-07-18',false,'Eum cum veritatis.'),
 (6,872.34,'2025-05-10',false,'Porro voluptatum sit voluptatem harum eaque quia.'),
 (6,663.83,'2025-07-09',true,'Velit quaerat placeat et veniam aut.'),
 (6,324.00,'2025-06-08',false,'Dolore quae consequuntur tenetur qui soluta rem a.'),
 (6,960.02,'2024-12-09',true,'Illo et laboriosam et.'),
 (6,115.86,'2025-01-28',false,'Quae ea expedita molestiae laboriosam quo molestiae voluptas in officiis.'),
 (6,460.42,'2025-07-11',true,'Ut natus vel temporibus.'),
 (6,627.01,'2025-04-09',true,'Dolor quos expedita sit optio.'),
 (6,380.03,'2024-10-02',true,'Esse ea est provident quisquam.'),
 (6,380.00,'2025-01-26',false,'Et autem earum fuga libero.');

INSERT INTO cobrancas (clienteid,valor,datavencimento,pago,descricao) VALUES
 (7,945.06,'2025-01-01',false,'Consectetur minima itaque numquam hic dolores.'),
 (7,380.17,'2025-03-20',true,'Minima quia adipisci recusandae enim assumenda ut suscipit.'),
 (7,283.56,'2025-07-15',true,'Tempora sint et recusandae officiis.'),
 (7,772.46,'2025-02-07',false,'Aut quod eos dolores quam quae minus perferendis ad ea.'),
 (7,854.10,'2025-03-18',false,'Similique voluptatem consequatur illo omnis aspernatur omnis quidem harum.'),
 (7,392.31,'2024-10-18',true,'Aspernatur animi deleniti facilis ut enim.'),
 (7,993.23,'2025-01-25',false,'Et quo assumenda et aut et voluptas est quasi.'),
 (7,765.10,'2025-03-20',false,'Sint sint sapiente exercitationem est doloribus eveniet nihil sapiente.'),
 (7,880.81,'2024-09-24',true,'Repudiandae cum quae id quam aperiam inventore minima et.'),
 (7,690.00,'2025-01-02',true,'Magnam esse cupiditate.');
	
INSERT INTO cobrancas (clienteid,valor,datavencimento,pago,descricao) VALUES
 (8,138.70,'2025-05-13',false,'Rerum id deserunt qui sit ducimus quo dolores enim.'),
 (8,371.28,'2025-04-16',true,'Ratione qui iste quis cumque temporibus nam consequatur optio.'),
 (8,651.52,'2025-07-15',false,'Incidunt doloribus praesentium repudiandae et sed libero tempore rem.'),
 (8,927.65,'2025-02-23',true,'Perferendis id et id odit et deleniti.'),
 (8,480.88,'2024-08-28',true,'Occaecati quia ex reprehenderit voluptates in.'),
 (8,811.82,'2025-02-12',true,'Occaecati possimus quia ratione asperiores quo at nihil.'),
 (8,155.54,'2025-03-05',true,'Vitae nemo doloribus amet aspernatur.'),
 (8,387.12,'2025-03-18',true,'Nobis explicabo ut in excepturi ad facere fugit quis quaerat.'),
 (8,912.11,'2024-12-24',false,'Enim facilis dolorum atque voluptatem id aut.'),
 (8,377.21,'2025-03-02',true,'Temporibus quis et eum delectus voluptate accusamus aut ratione fugiat.');
