#!/bin/bash
APP_NAME="./TypeScriptNative.app"
ENTITLEMENTS="./MyAppEntitlements.entitlements"
SIGNING_IDENTITY="Developer ID: TachyonVFX" # matches Keychain Access certificate name

find "$APP_NAME/Contents/MacOS"|while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$fname"
    fi
done

echo "[INFO] Signing app file"

codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$APP_NAME"