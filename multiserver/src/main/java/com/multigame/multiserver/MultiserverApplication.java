package com.multigame.multiserver;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;

@SpringBootApplication
@EntityScan(basePackages = "com.multigame.multiserver.member")
public class MultiserverApplication {

	public static void main(String[] args) {
		SpringApplication.run(MultiserverApplication.class, args);
	}
}
