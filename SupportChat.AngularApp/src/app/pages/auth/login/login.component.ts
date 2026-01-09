//@CodeCopy
import { Component } from '@angular/core';
import { AuthService } from '@app-services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  public email = '';
  public password = '';
  public error = '';
  public returnUrl = '/dashboard';
  public showPassword = false;
  public isLoading = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService) {

  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
    console.log('🔐 [LOGIN] ngOnInit - returnUrl:', this.returnUrl);

    if (!this.authService.isLoginRequired || this.authService.isLoggedIn) {
      console.log('🔐 [LOGIN] User bereits eingeloggt, navigiere zu:', this.returnUrl);
      this.router.navigateByUrl(this.returnUrl);
    }
  }

  public togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  public fillCredentials(email: string, password: string): void {
    this.email = email;
    this.password = password;
  }

  public async onLogin() {
    console.log('🔐 [LOGIN] Starte Login-Prozess...');
    console.log('🔐 [LOGIN] Email:', this.email);
    console.log('🔐 [LOGIN] Return URL:', this.returnUrl);
    
    this.error = '';
    this.isLoading = true;

    try {
      console.log('🔐 [LOGIN] Rufe authService.login() auf...');
      const user = await this.authService.login(this.email, this.password);
      console.log('🔐 [LOGIN] Login-Response erhalten:', user);

      if (user) {
        console.log('🔐 [LOGIN] User vorhanden, setze localStorage und navigiere zu:', this.returnUrl);
        localStorage.setItem('auth', 'true');
        this.router.navigateByUrl(this.returnUrl);
        console.log('🔐 [LOGIN] Navigation abgeschlossen');
      }
      else {
        console.error('🔐 [LOGIN] Kein User in Response!');
        this.error = 'Login fehlgeschlagen';
      }
    }
    catch (error) {
      console.error('🔐 [LOGIN] FEHLER beim Login:', error);
      if (error instanceof HttpErrorResponse) {
        console.error('🔐 [LOGIN] HTTP Error Status:', error.status);
        console.error('🔐 [LOGIN] HTTP Error Text:', error.statusText);
        console.error('🔐 [LOGIN] HTTP Error Message:', error.message);
        console.error('🔐 [LOGIN] HTTP Error Details:', error.error);
        this.error = `Login error: ${error.status} ${error.statusText}\n${error.message}`;
      }
      else {
        console.error('🔐 [LOGIN] Unbekannter Fehler:', error);
        this.error = 'Login error: An unknown error occurred.';
      }
    }
    finally {
      console.log('🔐 [LOGIN] Finally-Block erreicht, setze isLoading = false');
      this.isLoading = false;
    }
  }
}
