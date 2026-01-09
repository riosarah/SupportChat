//@CodeCopy
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '@app-services/auth.service';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpTokenInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {
  }

  intercept(request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    request = this.addToken(request);
    
    // Handle HTTP errors, especially 401 Unauthorized
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          console.error('🔒 [HTTP-INTERCEPTOR] 401 Unauthorized - Session abgelaufen');
          // Session abgelaufen - Logout durchführen
          this.authService.logout().then(() => {
            this.router.navigate(['/auth/login']);
          });
        }
        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>) {
    var sessionToken;

    if (this.authService.user && this.authService.user.sessionToken) {
      sessionToken = this.authService.user.sessionToken;
    }

    var authHeaderValue;

    if (sessionToken) {
      // Konvertiere Session-Token zu Base64 für Bearer Auth (gemäß FRONTEND_AUTHENTICATION.md)
      const base64Token = btoa(sessionToken);
      authHeaderValue = 'Bearer ' + base64Token;
      console.log('🔒 [HTTP-INTERCEPTOR] Bearer Token hinzugefügt (Base64-encoded)');
    }

    if (authHeaderValue) {
      const requestCopy = request.clone({
        headers: request.headers,
        setHeaders: {
          Authorization: authHeaderValue,
        },
      });

      return requestCopy;
    }
    else {
      return request;
    }
  }
}
