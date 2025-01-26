package com.multigame.multiserver;

import io.grpc.Context;

public class ContextKeys {
    private static Context.Key<String> USER_ID_CONTEXT_KEY;

    public static Context.Key<String> getUserIdContextKey() {
        if (USER_ID_CONTEXT_KEY == null) {
            USER_ID_CONTEXT_KEY = Context.key("userId");
        }
        return USER_ID_CONTEXT_KEY;
    }
}
