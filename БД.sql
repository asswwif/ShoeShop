CREATE DATABASE  IF NOT EXISTS `shoestore` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `shoestore`;
-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: localhost    Database: shoestore
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
-- Table structure for table `brand`
--

DROP TABLE IF EXISTS `brand`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `brand` (
  `brand_id` tinyint NOT NULL AUTO_INCREMENT,
  `brand_name` varchar(60) NOT NULL,
  `country` varchar(40) NOT NULL,
  PRIMARY KEY (`brand_id`),
  UNIQUE KEY `brand_id` (`brand_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `brand`
--

LOCK TABLES `brand` WRITE;
/*!40000 ALTER TABLE `brand` DISABLE KEYS */;
INSERT INTO `brand` VALUES (1,'Nike','USA'),(2,'Adidas','Germany'),(3,'New Balance','USA'),(4,'Puma','Germany'),(5,'Converse','USA'),(6,'Timberland','USA'),(7,'Birkenstock','Germany');
/*!40000 ALTER TABLE `brand` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `category_id` tinyint NOT NULL AUTO_INCREMENT,
  `category_name` varchar(40) NOT NULL,
  PRIMARY KEY (`category_id`),
  UNIQUE KEY `category_id` (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (1,'Кросівки'),(2,'Кеди низькі'),(3,'Кеди високі'),(4,'Балетки'),(5,'Футбольні бутси'),(6,'Черевики'),(7,'Шльопанці');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `color`
--

DROP TABLE IF EXISTS `color`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `color` (
  `color_id` tinyint NOT NULL AUTO_INCREMENT,
  `color_name` varchar(30) NOT NULL,
  PRIMARY KEY (`color_id`),
  UNIQUE KEY `color_id` (`color_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `color`
--

LOCK TABLES `color` WRITE;
/*!40000 ALTER TABLE `color` DISABLE KEYS */;
INSERT INTO `color` VALUES (1,'Чорний'),(2,'Білий'),(3,'Синій'),(4,'Червоний'),(5,'Зелений'),(6,'Жовтий'),(7,'Коричневий'),(8,'Сірий'),(9,'Рожевий'),(10,'Фіолетовий');
/*!40000 ALTER TABLE `color` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `color_size`
--

DROP TABLE IF EXISTS `color_size`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `color_size` (
  `color_size_id` tinyint NOT NULL AUTO_INCREMENT,
  `color_id` tinyint DEFAULT NULL,
  `size_id` tinyint DEFAULT NULL,
  `product_id` mediumint unsigned DEFAULT NULL,
  `stock_quantity` tinyint NOT NULL,
  PRIMARY KEY (`color_size_id`),
  UNIQUE KEY `color_size_id` (`color_size_id`),
  KEY `color_id` (`color_id`),
  KEY `size_id` (`size_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `color_size_ibfk_1` FOREIGN KEY (`color_id`) REFERENCES `color` (`color_id`) ON DELETE CASCADE,
  CONSTRAINT `color_size_ibfk_2` FOREIGN KEY (`size_id`) REFERENCES `size` (`size_id`) ON DELETE CASCADE,
  CONSTRAINT `color_size_ibfk_3` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=91 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `color_size`
--

LOCK TABLES `color_size` WRITE;
/*!40000 ALTER TABLE `color_size` DISABLE KEYS */;
INSERT INTO `color_size` VALUES (1,1,9,1,15),(2,1,10,1,12),(3,1,11,1,10),(4,2,9,1,20),(5,2,10,1,18),(6,2,11,1,14),(7,1,10,2,8),(8,1,11,2,7),(9,1,12,2,5),(10,2,10,2,10),(11,2,11,2,9),(12,2,12,2,6),(13,1,9,3,11),(14,1,10,3,10),(15,1,11,3,9),(16,8,9,3,15),(17,8,10,3,13),(18,8,11,3,12),(19,2,8,4,25),(20,2,9,4,22),(21,2,10,4,19),(22,1,8,4,18),(23,1,9,4,15),(24,1,10,4,12),(25,2,8,5,20),(26,2,9,5,16),(27,2,10,5,14),(28,5,8,5,10),(29,5,9,5,8),(30,5,10,5,6),(31,1,10,6,15),(32,1,11,6,12),(33,1,12,6,10),(34,3,10,6,13),(35,3,11,6,11),(36,3,12,6,9),(37,1,8,7,20),(38,1,9,7,18),(39,1,10,7,15),(40,2,8,7,25),(41,2,9,7,22),(42,2,10,7,18),(43,3,10,8,8),(44,3,11,8,6),(45,3,12,8,5),(46,1,10,8,10),(47,1,11,8,9),(48,1,12,8,7),(49,8,9,9,10),(50,8,10,9,10),(51,8,11,9,10),(52,3,9,9,9),(53,3,10,9,10),(54,3,11,9,8),(55,8,10,10,8),(56,8,11,10,7),(57,8,12,10,6),(58,1,10,10,10),(59,1,11,10,8),(60,1,12,10,7),(61,5,11,11,10),(62,5,12,11,8),(63,5,13,11,6),(64,4,11,11,9),(65,4,12,11,7),(66,4,13,11,5),(67,7,10,12,12),(68,7,11,12,10),(69,7,12,12,8),(70,1,10,12,9),(71,1,11,12,7),(72,1,12,12,5),(73,2,7,13,30),(74,2,8,13,25),(75,2,9,13,20),(76,1,7,13,15),(77,1,8,13,11),(78,1,9,13,10),(79,2,7,14,24),(80,2,8,14,20),(81,2,9,14,15),(82,1,7,14,10),(83,1,8,14,8),(84,1,9,14,6),(85,7,8,15,10),(86,7,9,15,8),(87,7,10,15,6),(88,6,8,15,9),(89,6,9,15,7),(90,6,10,15,5);
/*!40000 ALTER TABLE `color_size` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer`
--

DROP TABLE IF EXISTS `customer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer` (
  `customer_id` tinyint NOT NULL AUTO_INCREMENT,
  `first_name` varchar(30) NOT NULL,
  `last_name` varchar(30) NOT NULL,
  `phone_number` varchar(15) NOT NULL,
  `birth_date` date DEFAULT NULL,
  PRIMARY KEY (`customer_id`),
  UNIQUE KEY `customer_id` (`customer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer`
--

LOCK TABLES `customer` WRITE;
/*!40000 ALTER TABLE `customer` DISABLE KEYS */;
INSERT INTO `customer` VALUES (1,'Іван','Коваленко','+380501234566','1985-04-15'),(2,'Марія','Мельник','+380679876543','1992-08-22'),(5,'Дмитро','Бойко','+380997778899','1995-07-10'),(6,'Іван','Ковал','+380501234597','1986-04-15'),(9,'Антон','Антонов','+380666173569','2000-09-19'),(11,'Андрій','Антоно','+380666173568','1973-12-09'),(16,'Анна','Бойко','+380997778891','1996-07-10'),(17,'Марія','Бойчук','+380508141218','1997-10-02'),(18,'Андрій','Петренко','+380666173589','1999-11-03'),(24,'Ян','Яненко','+380501234567','2007-05-24');
/*!40000 ALTER TABLE `customer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `discount_card`
--

DROP TABLE IF EXISTS `discount_card`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `discount_card` (
  `discount_card_id` tinyint NOT NULL AUTO_INCREMENT,
  `customer_id` tinyint NOT NULL,
  `discount_percent` tinyint NOT NULL,
  PRIMARY KEY (`discount_card_id`),
  UNIQUE KEY `bonus_card_id` (`discount_card_id`),
  UNIQUE KEY `customer_id` (`customer_id`),
  CONSTRAINT `discount_card_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`customer_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `discount_card`
--

LOCK TABLES `discount_card` WRITE;
/*!40000 ALTER TABLE `discount_card` DISABLE KEYS */;
INSERT INTO `discount_card` VALUES (1,1,10),(2,2,15),(5,5,10),(7,9,20),(8,11,15),(9,6,5),(14,16,20),(15,17,20),(16,18,30);
/*!40000 ALTER TABLE `discount_card` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee` (
  `employee_id` tinyint NOT NULL AUTO_INCREMENT,
  `first_name` varchar(30) NOT NULL,
  `last_name` varchar(30) NOT NULL,
  `position` enum('Адміністратор','Продавець') NOT NULL,
  `phone_number` varchar(15) NOT NULL,
  `username` varchar(50) DEFAULT NULL,
  `password_hash` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`employee_id`),
  UNIQUE KEY `employee_id` (`employee_id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

LOCK TABLES `employee` WRITE;
/*!40000 ALTER TABLE `employee` DISABLE KEYS */;
INSERT INTO `employee` VALUES (1,'Іван','Петренко','Адміністратор','+380666173660','admin','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9'),(2,'Олена','Коваль','Продавець','+380666173546','seller1','2a76110d06bcc4fd437337b984131cfa82db9f792e3e2340acef9f3066b264e0'),(3,'Петро','Сидоренко','Продавець','+380503333333','seller2','fd7e66f59aa49da9fa3f2f64890916efe0602de2cc34594e4cc647030546be41'),(12,'','','Продавець','','aaaaaaaaaaaaaa','ed968e840d10d2d313a870bc131a4e2c311d7ad09bdf32b3418147221f51a6e2');
/*!40000 ALTER TABLE `employee` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `product_id` mediumint unsigned NOT NULL AUTO_INCREMENT,
  `product_name` varchar(100) NOT NULL,
  `article_number` varchar(25) NOT NULL,
  `category_id` tinyint DEFAULT NULL,
  `brand_id` tinyint DEFAULT NULL,
  `material` varchar(50) DEFAULT NULL,
  `price` float NOT NULL,
  `season` enum('Зима','Весна','Літо','Осінь') NOT NULL,
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `product_id` (`product_id`),
  UNIQUE KEY `article_number` (`article_number`),
  KEY `category_id` (`category_id`),
  KEY `brand_id` (`brand_id`),
  CONSTRAINT `product_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `category` (`category_id`) ON DELETE SET NULL,
  CONSTRAINT `product_ibfk_2` FOREIGN KEY (`brand_id`) REFERENCES `brand` (`brand_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
INSERT INTO `product` VALUES (1,'Air Force 1','AF1-001',1,1,'Шкіра',4000,'Весна'),(2,'Jordan Retro 4','JR4-002',1,1,'Шкіра',8500,'Весна'),(3,'Air Max 90','AM90-003',1,1,'Текстиль',4800,'Весна'),(4,'Superstar','SS-004',1,2,'Шкіра',3800,'Осінь'),(5,'Stan Smith','SSM-005',1,2,'Шкіра',3600,'Осінь'),(6,'Ultra Boost','UB-006',1,2,'Текстиль',7200,'Весна'),(7,'Chuck Taylor All Star','CTAS-007',2,5,'Текстиль',3600,'Весна'),(8,'Chuck 70 High','C70H-008',3,5,'Текстиль',3400,'Осінь'),(9,'574 Classic','NB574-009',1,3,'Замша',3200,'Осінь'),(10,'990v5','NB990-010',1,3,'Сітка',7000,'Весна'),(11,'Future Z','FZ-011',5,4,'Синтетика',6000,'Весна'),(12,'Timberland Original Yellow Boot','CLB-012',6,6,'Шкіра',4400,'Зима'),(13,'Adidas Adilette Slides','AAS-013',7,2,'Гума',2200,'Літо'),(14,'Converse Slides','CS-014',7,5,'Гума',2100,'Літо'),(15,'Birkenstock Arizona Sandals','LS-015',3,7,'Шкіра',6000,'Літо');
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `return`
--

DROP TABLE IF EXISTS `return`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `return` (
  `return_id` smallint NOT NULL AUTO_INCREMENT,
  `sale_detail_id` tinyint NOT NULL,
  `employee_id` tinyint NOT NULL,
  `return_date` date NOT NULL,
  `product_quantity` tinyint NOT NULL,
  `reason` text NOT NULL,
  PRIMARY KEY (`return_id`),
  UNIQUE KEY `return_id` (`return_id`),
  KEY `order_detail_id` (`sale_detail_id`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `return_ibfk_1` FOREIGN KEY (`sale_detail_id`) REFERENCES `sale_details` (`sale_detail_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `return_ibfk_2` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `return`
--

LOCK TABLES `return` WRITE;
/*!40000 ALTER TABLE `return` DISABLE KEYS */;
INSERT INTO `return` VALUES (1,5,1,'2025-09-28',1,'Не підійшов розмір'),(2,2,2,'2025-09-28',1,'Брак'),(3,1,3,'2025-09-27',1,'-'),(4,4,1,'2025-09-28',1,'Не вказано'),(5,9,1,'2025-09-28',1,'Не підійшов розмір'),(6,1,1,'2025-09-28',1,'-'),(7,16,1,'2025-10-01',3,'Не підійшов розмір'),(8,3,1,'2025-10-11',1,'Не вказано'),(9,10,1,'2025-10-17',1,'Не вказано');
/*!40000 ALTER TABLE `return` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sale`
--

DROP TABLE IF EXISTS `sale`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sale` (
  `sale_id` tinyint NOT NULL AUTO_INCREMENT,
  `customer_id` tinyint DEFAULT NULL,
  `sale_datetime` datetime NOT NULL,
  `sale_total` float NOT NULL,
  `payment_status` enum('Оплачено','Очікує','Відхилено') NOT NULL,
  `employee_id` tinyint DEFAULT NULL,
  PRIMARY KEY (`sale_id`),
  UNIQUE KEY `order_id` (`sale_id`),
  KEY `customer_id` (`customer_id`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `sale_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`customer_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `sale_ibfk_2` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sale`
--

LOCK TABLES `sale` WRITE;
/*!40000 ALTER TABLE `sale` DISABLE KEYS */;
INSERT INTO `sale` VALUES (1,2,'2025-09-28 09:36:34',2720,'Оплачено',1),(2,2,'2025-09-28 09:47:59',2720,'Оплачено',1),(3,NULL,'2025-09-28 10:00:10',3200,'Оплачено',1),(4,5,'2025-09-28 10:06:11',5760,'Оплачено',1),(5,NULL,'2025-09-28 10:08:20',6400,'Оплачено',1),(6,NULL,'2025-09-28 10:28:38',3200,'Оплачено',1),(7,NULL,'2025-09-28 10:33:24',3200,'Оплачено',1),(8,NULL,'2025-09-28 10:55:00',3200,'Оплачено',1),(9,NULL,'2025-09-28 10:59:31',3200,'Оплачено',1),(10,NULL,'2025-09-28 11:04:51',2100,'Оплачено',1),(11,NULL,'2025-09-28 11:18:01',3200,'Оплачено',1),(12,NULL,'2025-09-28 11:30:48',2200,'Оплачено',1),(13,NULL,'2025-09-28 11:49:55',3200,'Оплачено',1),(14,6,'2025-10-01 13:22:23',9120,'Оплачено',1),(15,17,'2025-10-11 11:06:41',2560,'Оплачено',1),(16,9,'2025-10-11 14:05:48',2560,'Оплачено',NULL),(17,NULL,'2025-10-11 14:11:26',3600,'Оплачено',NULL),(18,18,'2025-10-11 19:25:56',3200,'Оплачено',NULL),(19,NULL,'2025-10-12 15:45:44',3600,'Оплачено',1);
/*!40000 ALTER TABLE `sale` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sale_details`
--

DROP TABLE IF EXISTS `sale_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sale_details` (
  `sale_detail_id` tinyint NOT NULL AUTO_INCREMENT,
  `sale_id` tinyint NOT NULL,
  `color_size_id` tinyint NOT NULL,
  `product_quantity` tinyint NOT NULL,
  `unit_price` float NOT NULL,
  PRIMARY KEY (`sale_detail_id`),
  UNIQUE KEY `order_detail_id` (`sale_detail_id`),
  KEY `order_id` (`sale_id`),
  KEY `color_size_id` (`color_size_id`),
  CONSTRAINT `sale_details_ibfk_1` FOREIGN KEY (`sale_id`) REFERENCES `sale` (`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `sale_details_ibfk_2` FOREIGN KEY (`color_size_id`) REFERENCES `color_size` (`color_size_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sale_details`
--

LOCK TABLES `sale_details` WRITE;
/*!40000 ALTER TABLE `sale_details` DISABLE KEYS */;
INSERT INTO `sale_details` VALUES (1,1,52,1,3200),(2,2,52,1,3200),(3,3,53,1,3200),(4,4,53,1,3200),(5,4,49,1,3200),(6,5,52,1,3200),(7,5,49,1,3200),(8,6,50,1,3200),(9,7,51,1,3200),(10,8,54,1,3200),(11,9,49,1,3200),(12,10,79,1,2100),(13,11,50,1,3200),(14,12,77,1,2200),(15,13,50,1,3200),(16,14,49,3,3200),(17,15,49,1,3200),(18,16,49,1,3200),(19,17,26,1,3600),(20,18,52,1,3200),(21,19,42,1,3600);
/*!40000 ALTER TABLE `sale_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `size`
--

DROP TABLE IF EXISTS `size`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `size` (
  `size_id` tinyint NOT NULL AUTO_INCREMENT,
  `size_value` varchar(10) NOT NULL,
  PRIMARY KEY (`size_id`),
  UNIQUE KEY `size_id` (`size_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `size`
--

LOCK TABLES `size` WRITE;
/*!40000 ALTER TABLE `size` DISABLE KEYS */;
INSERT INTO `size` VALUES (1,'32'),(2,'33'),(3,'34'),(4,'35'),(5,'36'),(6,'37'),(7,'38'),(8,'39'),(9,'40'),(10,'41'),(11,'42'),(12,'43'),(13,'44'),(14,'45'),(15,'46');
/*!40000 ALTER TABLE `size` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-10-28 14:41:48
