import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { Router } from '@angular/router';

import { ISupportTicket } from '@app-models/entities/app/i-support-ticket';
import { IChatMessage } from '@app-models/entities/app/i-chat-message';
import { SupportTicketService } from '@app-services/http/entities/app/support-ticket-service';
import { ChatMessageService } from '@app-services/http/entities/app/chat-message-service';
import { AuthService } from '@app-services/auth.service';
import { TicketStatus } from '@app-enums/app/ticket-status';
import { TicketPriority } from '@app-enums/app/ticket-priority';
import { Nl2brPipe } from '@app/pipes/nl2br.pipe';

@Component({
  selector: 'app-chat-history',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, Nl2brPipe],
  templateUrl: './chat-history.component.html',
  styleUrls: ['./chat-history.component.css']
})
export class ChatHistoryComponent implements OnInit {
  public tickets: ISupportTicket[] = [];
  public filteredTickets: ISupportTicket[] = [];
  public selectedTicket: ISupportTicket | null = null;
  public messages: IChatMessage[] = [];
  public isLoading: boolean = true;
  public isLoadingMessages: boolean = false;
  public filterStatus: string = 'all';

  // Enums für Template-Zugriff
  public readonly TicketStatus = TicketStatus;
  public readonly TicketPriority = TicketPriority;

  constructor(
    private supportTicketService: SupportTicketService,
    private chatMessageService: ChatMessageService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/auth/login']);
      return;
    }

    this.loadTickets();
  }

  private loadTickets(): void {
    const identityId = this.authService.user?.identityId;
    
    if (!identityId) {
      console.error('🎫 [HISTORY] Keine Identity ID gefunden!');
      return;
    }

    this.isLoading = true;
    
    this.supportTicketService.getAll().subscribe({
      next: (allTickets) => {
        // Filter tickets by current user
        this.tickets = allTickets.filter(t => t.identityId === identityId);
        this.tickets.sort((a, b) => new Date(b.startTime).getTime() - new Date(a.startTime).getTime());
        this.applyFilter();
        this.isLoading = false;
        
        console.log('🎫 [HISTORY] Tickets geladen:', this.tickets.length);
      },
      error: (error) => {
        console.error('🎫 [HISTORY] Fehler beim Laden:', error);
        this.isLoading = false;
      }
    });
  }

  public applyFilter(): void {
    if (this.filterStatus === 'all') {
      this.filteredTickets = this.tickets;
    } else if (this.filterStatus === 'resolved') {
      this.filteredTickets = this.tickets.filter(t => t.status === TicketStatus.Resolved);
    } else if (this.filterStatus === 'unresolved') {
      this.filteredTickets = this.tickets.filter(t => t.status === TicketStatus.Unresolved);
    }
  }

  public selectTicket(ticket: ISupportTicket): void {
    this.selectedTicket = ticket;
    this.loadMessages(ticket.id);
  }

  private loadMessages(ticketId: number): void {
    this.isLoadingMessages = true;
    this.messages = [];
    
    this.chatMessageService.getAll().subscribe({
      next: (allMessages) => {
        this.messages = allMessages
          .filter(m => m.supportTicketId === ticketId)
          .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
        
        this.isLoadingMessages = false;
        console.log('💬 [HISTORY] Nachrichten geladen:', this.messages.length);
      },
      error: (error) => {
        console.error('💬 [HISTORY] Fehler beim Laden:', error);
        this.isLoadingMessages = false;
      }
    });
  }

  public closeDetail(): void {
    this.selectedTicket = null;
    this.messages = [];
  }

  public getStatusClass(status: TicketStatus): string {
    return status === TicketStatus.Resolved ? 'status-resolved' : 'status-unresolved';
  }

  public getPriorityClass(priority: TicketPriority): string {
    switch (priority) {
      case TicketPriority.Critical:
        return 'priority-critical';
      case TicketPriority.High:
        return 'priority-high';
      case TicketPriority.Medium:
        return 'priority-medium';
      case TicketPriority.Low:
        return 'priority-low';
      default:
        return 'priority-medium';
    }
  }

  public formatDate(date: Date): string {
    return new Date(date).toLocaleString('de-DE', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  public formatTime(date: Date): string {
    return new Date(date).toLocaleTimeString('de-DE', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  public updateStatus(ticket: ISupportTicket, newStatus: TicketStatus): void {
    if (ticket.status === newStatus) return;

    console.log('🔄 [HISTORY] Status-Update gestartet:', { ticketId: ticket.id, newStatus });

    this.supportTicketService.updateStatus(ticket.id, newStatus).subscribe({
      next: (updatedTicket: ISupportTicket) => {
        ticket.status = updatedTicket.status;
        if (updatedTicket.endTime) {
          ticket.endTime = updatedTicket.endTime;
        }
        console.log('✅ [HISTORY] Status aktualisiert:', ticket.id, '→', newStatus);
        this.applyFilter(); // Filter neu anwenden
      },
      error: (error: any) => {
        console.error('❌ [HISTORY] Fehler beim Status-Update:', error);
        if (error.error) {
          console.error('❌ [HISTORY] Backend-Fehler:', error.error);
        }
        alert('Fehler beim Aktualisieren des Status: ' + (error.error?.message || error.message || 'Unbekannter Fehler'));
      }
    });
  }

  public updatePriority(ticket: ISupportTicket, newPriority: TicketPriority): void {
    if (ticket.priority === newPriority) return;

    console.log('🔄 [HISTORY] Prioritäts-Update gestartet:', { ticketId: ticket.id, newPriority });

    this.supportTicketService.updatePriority(ticket.id, newPriority).subscribe({
      next: (updatedTicket: ISupportTicket) => {
        ticket.priority = updatedTicket.priority;
        console.log('✅ [HISTORY] Priorität aktualisiert:', ticket.id, '→', newPriority);
      },
      error: (error: any) => {
        console.error('❌ [HISTORY] Fehler beim Prioritäts-Update:', error);
        if (error.error) {
          console.error('❌ [HISTORY] Backend-Fehler:', error.error);
        }
        alert('Fehler beim Aktualisieren der Priorität: ' + (error.error?.message || error.message || 'Unbekannter Fehler'));
      }
    });
  }
}
