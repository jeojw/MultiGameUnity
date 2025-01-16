package com.multigame.multiserver.auth;

import io.jsonwebtoken.*;
import io.jsonwebtoken.security.Keys;
import lombok.extern.log4j.Log4j2;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Component;

import java.security.Key;
import java.util.Date;
import java.util.concurrent.TimeUnit;

@Component
@Log4j2
public class JwtUtil {
    @Autowired
    private RedisTemplate<String, String> redisTemplate;

    @Value("${spring.jwt.secret}")
    private static String secretKey;

    @Value("${spring.jwt.token.access-expiration-time}")
    private static long accessExpirationTime;

    @Value("${spring.jwt.token.refresh-expiration-time}")
    private static long refreshExpirationTime;

    public String generateAccessToken(String userId) {
        Claims claims = Jwts.claims().setSubject(userId);
        Date now = new Date();
        Date expireDate = new Date(now.getTime() + accessExpirationTime);
        Key key = Keys.hmacShaKeyFor(secretKey.getBytes());

        return Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(now)
                .setExpiration(expireDate)
                .signWith(key, SignatureAlgorithm.HS256)
                .compact();
    }

    public String generateRefreshToken(String userId) {
        Claims claims = Jwts.claims().setSubject(userId);
        Date now = new Date();
        Date expireDate = new Date(now.getTime() + refreshExpirationTime);
        Key key = Keys.hmacShaKeyFor(secretKey.getBytes());

        String refreshToken = Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(now)
                .setExpiration(expireDate)
                .signWith(key, SignatureAlgorithm.HS256)
                .compact();

        redisTemplate.opsForValue().set(
                userId,
                refreshToken,
                refreshExpirationTime,
                TimeUnit.MILLISECONDS
        );

        return refreshToken;
    }

    public String getUserIdFromToken(String token) {
        Key key = Keys.hmacShaKeyFor(secretKey.getBytes());

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

    public boolean validateToken(String token) {
        try {
            Key key = Keys.hmacShaKeyFor(secretKey.getBytes());

            Jwts.parserBuilder().setSigningKey(key).build().parseClaimsJws(token);
            return true;
        } catch (ExpiredJwtException e) {
            throw new RuntimeException(e.getMessage());
        } catch (JwtException e) {
            throw new RuntimeException(e.getMessage());
        }
    }
}
