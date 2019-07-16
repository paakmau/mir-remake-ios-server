-- create database legend;
use legend;

DROP TABLE IF EXISTS `character`;
CREATE TABLE `character` (
  `characterid` int NOT NULL AUTO_INCREMENT,
  `playerid` int NOT NULL,
  `occupation` VARCHAR(255) NOT NULL,
  `level` int DEFAULT NULL,
  `experience` int DEFAULT NULL,
  `currency` varchar(255) NOT NULL,
  `giftpoints` varchar(255) NOT NULL,
  `name` varchar(255) DEFAULT NULL,
   primary key (`characterid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE INDEX `playedid` ON `character`(`playerid`);

DROP TABLE IF EXISTS `skill`;
create table `skill`( 
  `realid` int NOT NULL AUTO_INCREMENT,
  `skillid` int NOT NULL, 
  `charid` int NOT NULL, 
  `masterly` int NOT NULL ,
  `level` int NOT NULL, 
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE INDEX `charid` ON `skill`(`charid`);

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

CREATE INDEX `charid` ON `item`(`charid`);

DROP TABLE IF EXISTS `equipment`;
CREATE table `equipment`(
  `realid` int not null AUTO_INCREMENT,
  `charid` int not null,
  `strength_num` int not null,
  `gem_list` VARCHAR(255) NOT NULL,
  `enchant_attr` VARCHAR(255) NOT NULL,
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE INDEX `charid` ON `equipment`(`charid`);

DROP TABLE IF EXISTS `mission`;
create table `mission`(
  `realid` int NOT NULL AUTO_INCREMENT,
  `missionid` int NOT NULL,
  `charid` int NOT NULL,
  `targets` VARCHAR(150) NOT NULL,
  `status` VARCHAR(255) NOT NULL,
   primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE INDEX `charid` ON `mission`(`charid`);

DROP TABLE IF EXISTS `character_position`;
CREATE TABLE `character_position` (
  `charid` int NOT NULL AUTO_INCREMENT,
  `x` VARCHAR(20),
  `y` VARCHAR(20),
   primary key (`charid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `user`;
create table `user`( 
  `userid` int NOT NULL AUTO_INCREMENT,
  `user_name` VARCHAR(155) NOT NULL,
  `password` VARCHAR(155) NOT NULL,
  `question` VARCHAR(255),
  `answer` VARCHAR(255),
  primary key (`userid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE UNIQUE INDEX `user_name` ON `user`(`user_name`);

DROP TABLE IF EXISTS `vip`;
create table `vip`( 
  `userid` int NOT NULL AUTO_INCREMENT,
  `vip_level` int NOT NULL,
  `charge_money` int NOT NULL,
  primary key (`userid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `enchantment`;
create table `enchantment`( 
  `realid` int NOT NULL AUTO_INCREMENT,
  `charid` int NOT NULL,
  `enchant_attr` VARCHAR(255),
  primary key (`realid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE INDEX `charid` ON `enchantment`(`charid`);

DROP TABLE IF EXISTS `combat_effect`;
create table `combat_effect`( 
  `charid` int NOT NULL,
  `combat` int,
  `name` VARCHAR(45) NOT NULL,
  `occupation` VARCHAR(45) NOT NULL,
  primary key (`charid`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;