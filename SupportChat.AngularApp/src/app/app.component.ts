import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  public title = 'SEMusicStoreAngular-Developer';
  public currentLanguage = 'de';
  public currentTheme: 'light' | 'dark' = 'light';
  public isMenuCollapsed = true;

  public get isLoginRequired(): boolean {
    const value = this.authService.isLoginRequired;
    console.log('✅ [APP] isLoginRequired:', value);
    return value;
  }
  public get isLoggedIn(): boolean {
    const value = this.authService.isLoggedIn;
    console.log('✅ [APP] isLoggedIn:', value);
    return value;
  }
  public get userName(): string {
    const value = this.authService.user?.name || '';
    console.log('✅ [APP] userName:', value);
    return value;
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private translateService: TranslateService) {
    
    // Initiale Sprache setzen nach kurzer Verzögerung
    setTimeout(() => {
      this.currentLanguage = this.translateService.currentLang || 'de';
    }, 50);
    
    // Theme aus localStorage laden oder Standard verwenden
    const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | null;
    this.currentTheme = savedTheme || 'light';
    this.applyTheme(this.currentTheme);
    
    // Auf Sprachwechsel reagieren
    this.translateService.onLangChange.subscribe((event) => {
      this.currentLanguage = event.lang;
    });
  }

  public switchLanguage(language: string) {
    console.log('Switching to language:', language); // Debug
    this.translateService.use(language).subscribe(() => {
      this.currentLanguage = language;
      console.log('Language switched to:', this.currentLanguage); // Debug
    });
  }

  public async logout() {
    console.log('🚪 [APP] Logout initiiert');
    await this.authService.logout();
    console.log('🚪 [APP] Logout abgeschlossen, navigiere zu /auth/login');
    this.router.navigate(['/auth/login']);
  }

  public toggleTheme() {
    this.currentTheme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.applyTheme(this.currentTheme);
    localStorage.setItem('theme', this.currentTheme);
  }

  private applyTheme(theme: 'light' | 'dark') {
    document.documentElement.setAttribute('data-bs-theme', theme);
  }
}
