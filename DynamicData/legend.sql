-- create database legend;
use legend;

DROP TABLE IF EXISTS `character`;
CREATE TABLE `character` (
  `characterid` int NOT NULL AUTO_INCREMENT,
  `occupation` VARCHAR(255) NOT NULL,
  `level` int DEFAULT NULL,
  `experience` int DEFAULT NULL,
  `currency` varchar(255) NOT NULL,
  `giftpoints` varchar(255) NOT NULL,
   primary key (`characterid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `skill`;
create table `skill`( 
  `realid` int NOT NULL AUTO_INCREMENT,
  `skillid` int NOT NULL, 
  `charid` int NOT NULL, 
  `masterly` int NOT NULL ,
  `level` int NOT NULL, 
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item`;
create table `item`(
  `realid` int NOT NULL AUTO_INCREMENT,
  `itemid` int NOT NULL,
  `charid` int NOT NULL,
  `num` int NOT NULL,
  `place` VARCHAR(45) NOT NULL,
  `position` int NOT NULL,
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `equipment`;
CREATE table `equipment`(
  `realid` int not null AUTO_INCREMENT,
  `charid` int not null,
  `strength_num` int not null,
  `gem_list` VARCHAR(255) NOT NULL,
  `enchant_attr` VARCHAR(255) NOT NULL,
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `mission`;
create table `mission`(
  `realid` int NOT NULL AUTO_INCREMENT,
  `missionid` int NOT NULL,
  `charid` int NOT NULL,
  `targets` VARCHAR(150) NOT NULL,
  `status` VARCHAR(255) NOT NULL,
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
DROP TABLE IF EXISTS `character_position`;
CREATE TABLE `character` (
  `charid` int NOT NULL,
  `x` VARCHAR(20),
  `y` VARCHAR(20),
   primary key (`characterid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;