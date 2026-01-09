import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './pages/auth/login/login.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ChatbotComponent } from './pages/chatbot/chatbot.component';
import { ChatHistoryComponent } from './pages/chat-history/chat-history.component';

const routes: Routes = [
  // Öffentlicher Login-Bereich
  { path: 'auth/login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'chatbot', component: ChatbotComponent, canActivate: [AuthGuard] },
  { path: 'chat-history', component: ChatHistoryComponent, canActivate: [AuthGuard] },

  // Geschützter Bereich mit Dashboard und Unterseiten
  //{ path: 'protected', component: ProtectedListComponent, canActivate: [AuthGuard] },

  // Redirect von leerem Pfad auf Login
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },

  // Fallback bei ungültiger URL
  { path: '**', redirectTo: '/auth/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
