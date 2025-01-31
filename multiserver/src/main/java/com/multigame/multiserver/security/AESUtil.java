package com.multigame.multiserver.security;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.security.SecureRandom;
import java.util.Arrays;
import java.util.Base64;

@Component
public class AESUtil {
    @Value("${encryption.aes.key}")
    private String KEY;


    public String encrypt(String plaintext) throws Exception {
        byte[] keyBytes = Base64.getDecoder().decode(KEY);
        SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");

        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");
        byte[] ivBytes = new byte[16];
        new SecureRandom().nextBytes(ivBytes);
        IvParameterSpec ivSpec = new IvParameterSpec(ivBytes);
        cipher.init(Cipher.ENCRYPT_MODE, keySpec, ivSpec);

        byte[] encrypted = cipher.doFinal(plaintext.getBytes());

        byte[] encryptedWithIv = new byte[ivBytes.length + encrypted.length];
        System.arraycopy(ivBytes, 0, encryptedWithIv, 0, ivBytes.length);
        System.arraycopy(encrypted, 0, encryptedWithIv, ivBytes.length, encrypted.length);

        return Base64.getEncoder().encodeToString(encryptedWithIv);
    }
    
    public String decrypt(String encrypted) throws Exception {
        byte[] decoded = Base64.getDecoder().decode(encrypted);

        byte[] ivBytes = Arrays.copyOfRange(decoded, 0, 16);
        byte[] encryptedBytes = Arrays.copyOfRange(decoded, 16, decoded.length);

        byte[] keyBytes = Base64.getDecoder().decode(KEY);
        SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");

        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");
        IvParameterSpec ivSpec = new IvParameterSpec(ivBytes);
        cipher.init(Cipher.DECRYPT_MODE, keySpec, ivSpec);

        byte[] original = cipher.doFinal(encryptedBytes);
        return new String(original);
    }
}
