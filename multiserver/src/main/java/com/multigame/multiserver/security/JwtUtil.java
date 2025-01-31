package com.multigame.multiserver.security;

import io.jsonwebtoken.*;
import io.jsonwebtoken.security.Keys;
import lombok.extern.log4j.Log4j2;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.security.Key;
import java.util.Base64;
import java.util.Date;
import java.util.Objects;
import java.util.UUID;

@Component
@Log4j2
public class JwtUtil {
    @Value("${spring.jwt.secret}")
    private String userSecretKey;

    @Value("${spring.jwt.secret.fusion}")
    private String fusionSecretKey;

    @Value("${spring.jwt.token.access-expiration-time}")
    private long accessExpirationTime;

    @Value("${spring.jwt.token.refresh-expiration-time}")
    private long refreshExpirationTime;

    @Autowired
    private AESUtil aesUtil;

    public String generateAccessToken(String userId) {
        Claims claims = Jwts.claims().setSubject(userId)
                .setId(UUID.randomUUID().toString());
        Date now = new Date();
        Date expireDate = new Date(now.getTime() + accessExpirationTime);
        Key key;
        if (!Objects.equals(userId, "fusion")) {
            key = Keys.hmacShaKeyFor(userSecretKey.getBytes());
        }
        else {
            key = Keys.hmacShaKeyFor(fusionSecretKey.getBytes());
        }

        return Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(now)
                .setExpiration(expireDate)
                .signWith(key, SignatureAlgorithm.HS256)
                .compact();
    }

    public String generateRefreshToken(String userId) {
        Claims claims = Jwts.claims().setSubject(userId)
                .setId(UUID.randomUUID().toString());
        Date now = new Date();
        Date expireDate = new Date(now.getTime() + refreshExpirationTime);
        Key key;
        if (!Objects.equals(userId, "fusion")) {
            key = Keys.hmacShaKeyFor(userSecretKey.getBytes());
        }
        else {
            key = Keys.hmacShaKeyFor(fusionSecretKey.getBytes());
        }

        return Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(now)
                .setExpiration(expireDate)
                .signWith(key, SignatureAlgorithm.HS256)
                .compact();
    }

    public String getUserIdFromToken(String token) {
        Key key = Keys.hmacShaKeyFor(userSecretKey.getBytes());

        try {
            Claims claims = Jwts.parserBuilder()
                    .setSigningKey(key)
                    .build()
                    .parseClaimsJws(token)
                    .getBody();

            return claims.getSubject();
        } catch (JwtException e) {
            throw new RuntimeException(e);
        }
    }

    public String getFusionFromToken(String token) {
        Key key = Keys.hmacShaKeyFor(fusionSecretKey.getBytes());

        try {
            Claims claims = Jwts.parserBuilder()
                    .setSigningKey(key)
                    .build()
                    .parseClaimsJws(token)
                    .getBody();

            return claims.getSubject();
        } catch (JwtException e) {
            throw new RuntimeException(e);
        }
    }
}
