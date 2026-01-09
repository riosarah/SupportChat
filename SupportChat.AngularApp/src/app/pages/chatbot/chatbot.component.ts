import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { Router } from '@angular/router';

import { ISupportTicket } from '@app-models/entities/app/i-support-ticket';
import { IChatMessage } from '@app-models/entities/app/i-chat-message';
import { SupportTicketService, ISendMessageRequest } from '@app-services/http/entities/app/support-ticket-service';
import { ChatMessageService } from '@app-services/http/entities/app/chat-message-service';
import { TicketStatus } from '@app-enums/app/ticket-status';
import { TicketPriority } from '@app-enums/app/ticket-priority';
import { MessageBoxService } from '@app-services/message-box-service.service';
import { AuthService } from '@app-services/auth.service';
import { Nl2brPipe } from '@app/pipes/nl2br.pipe';

@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, Nl2brPipe],
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit, OnDestroy {
  @ViewChild('chatContainer') private chatContainer!: ElementRef;
  @ViewChild('messageInput') private messageInput!: ElementRef;

  public currentTicket: ISupportTicket | null = null;
  public messages: IChatMessage[] = [];
  public userMessage: string = '';
  public isLoading: boolean = false;
  public isChatActive: boolean = false;
  public systemInfo: string = '';

  constructor(
    private supportTicketService: SupportTicketService,
    private chatMessageService: ChatMessageService,
    private messageBoxService: MessageBoxService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Prüfe ob Benutzer eingeloggt ist
    if (!this.authService.isLoggedIn) {
      console.error('🤖 [CHATBOT] Benutzer nicht eingeloggt!');
      this.messageBoxService.show('Bitte melden Sie sich an, um den Chatbot zu nutzen.', 'Fehler');
      this.router.navigate(['/auth/login']);
      return;
    }

    console.log('🤖 [CHATBOT] Benutzer eingeloggt:', this.authService.user?.name);
    console.log('🤖 [CHATBOT] Identity ID:', this.authService.user?.identityId);
    
    // Automatisch System-Info sammeln
    this.collectSystemInfo();
  }

  ngOnDestroy(): void {
    // Cleanup wenn nötig
  }

  private collectSystemInfo(): void {
    const userAgent = navigator.userAgent;
    const platform = navigator.platform;
    const language = navigator.language;
    
    this.systemInfo = `Browser: ${userAgent}\nPlattform: ${platform}\nSprache: ${language}`;
  }

  public startNewChat(): void {
    this.isChatActive = true;
    this.messages = [];
    this.currentTicket = null;
    
    // Begrüßungsnachricht vom Bot
    const welcomeMessage: IChatMessage = {
      id: 0,
      content: 'Hallo! Ich bin Ihr IT-Support-Assistent. Wie kann ich Ihnen heute helfen?',
      isUserMessage: false,
      timestamp: new Date(),
      supportTicketId: 0,
      supportTicket: null
    };
    this.messages.push(welcomeMessage);
    
    setTimeout(() => this.scrollToBottom(), 100);
  }

  private async createTicketFirst(problemDescription: string, identityId: number): Promise<void> {
    return new Promise((resolve, reject) => {
      this.supportTicketService.getTemplate().subscribe({
        next: (template) => {
          const newTicket: ISupportTicket = {
            ...template,
            problemDescription: problemDescription,
            systemInfo: this.systemInfo,
            status: TicketStatus.Unresolved,
            priority: TicketPriority.Medium,
            startTime: new Date(),
            endTime: null,
            recommendation: '',
            identityId: identityId,
            ticketSteps: [],
            chatMessages: [],
            chatResponses: []
          };
          
          console.log('🎫 [CHATBOT] Erstelle Ticket:', newTicket);

          this.supportTicketService.create(newTicket).subscribe({
            next: (createdTicket) => {
              this.currentTicket = createdTicket;
              console.log('✓ [CHATBOT] Ticket erstellt mit ID:', createdTicket.id);
              resolve();
            },
            error: (error) => {
              console.error('✗ [CHATBOT] Fehler beim Erstellen des Tickets:', error);
              reject(error);
            }
          });
        },
        error: (error) => {
          console.error('✗ [CHATBOT] Fehler beim Holen des Templates:', error);
          reject(error);
        }
      });
    });
  }

  public async sendMessage(): Promise<void> {
    if (!this.userMessage.trim() || this.isLoading) {
      return;
    }

    const messageContent = this.userMessage.trim();
    this.userMessage = '';
    this.isLoading = true;

    // Benutzernachricht zur Anzeige hinzufügen
    const userMsg: IChatMessage = {
      id: 0,
      content: messageContent,
      isUserMessage: true,
      timestamp: new Date(),
      supportTicketId: this.currentTicket?.id || 0,
      supportTicket: null
    };
    this.messages.push(userMsg);
    this.scrollToBottom();

    try {
      // Hole Identity ID aus AuthService
      const identityId = this.authService.user?.identityId;
      
      if (!identityId) {
        console.error('🤖 [CHATBOT] Keine Identity ID gefunden!');
        throw new Error('Keine gültige Benutzer-Session gefunden.');
      }

      // Wenn noch kein Ticket existiert, erstelle es zuerst
      if (!this.currentTicket) {
        console.log('🎫 [CHATBOT] Erstelle neues Ticket...');
        await this.createTicketFirst(messageContent, identityId);
      }

      // Jetzt haben wir eine gültige Ticket-ID
      const ticketId = this.currentTicket!.id;

      // Bereite Request vor
      const request: ISendMessageRequest = {
        content: messageContent,
        identityId: identityId
      };

      console.log('📤 [CHATBOT] Sende Nachricht an Ticket:', ticketId);

      // Nutze den neuen Endpoint
      this.supportTicketService.sendMessage(ticketId, request).subscribe({
        next: (chatResponse) => {
          console.log('✓ [CHATBOT] Antwort erhalten:', chatResponse);

          // Bot-Nachricht zur Anzeige hinzufügen
          const botMsg: IChatMessage = {
            id: chatResponse.botMessageId || 0,
            content: chatResponse.responseContent,
            isUserMessage: false,
            timestamp: chatResponse.createdAt,
            supportTicketId: ticketId,
            supportTicket: null
          };
          this.messages.push(botMsg);

          // Aktualisiere currentTicket wenn es ein neues Ticket war (sollte bereits gesetzt sein)
          if (!this.currentTicket) {
            this.currentTicket = {
              id: ticketId,
              problemDescription: messageContent,
              systemInfo: this.systemInfo,
              status: TicketStatus.Unresolved,
              priority: TicketPriority.Medium,
              startTime: new Date(),
              endTime: null,
              recommendation: '',
              identityId: identityId,
              ticketSteps: [],
              chatMessages: [],
              chatResponses: []
            };
          }

          this.scrollToBottom();
        },
        error: (error) => {
          console.error('✗ [CHATBOT] Fehler beim Senden der Nachricht:', error);
          
          // Extrahiere Fehlernachricht
          let errorMessage = 'Ein unbekannter Fehler ist aufgetreten.';
          if (error?.error?.error) {
            errorMessage = error.error.error;
          } else if (error?.message) {
            errorMessage = error.message;
          } else if (error?.statusText) {
            errorMessage = error.statusText;
          }
          
          this.messageBoxService.show('Fehler beim Senden der Nachricht: ' + errorMessage, 'Fehler');
          
          // Entferne die fehlerhafte Benutzernachricht aus der Anzeige
          this.messages = this.messages.filter(m => m !== userMsg);
          
          // Wichtig: isLoading zurücksetzen, damit weitere Versuche möglich sind
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        }
      });
    } catch (error: any) {
      console.error('✗ [CHATBOT] Fehler:', error);
      this.messageBoxService.show('Fehler beim Senden der Nachricht', 'Fehler');
      this.messages = this.messages.filter(m => m !== userMsg);
      this.isLoading = false;
    }
  }

  public endChat(): void {
    if (!this.currentTicket) {
      this.isChatActive = false;
      this.messages = [];
      return;
    }

    this.messageBoxService.confirm(
      'Möchten Sie den Chat beenden?',
      'Chat beenden'
    ).then((confirmed: boolean) => {
      if (confirmed && this.currentTicket) {
        // Ticket als gelöst markieren
        const updatedTicket = {
          ...this.currentTicket,
          endTime: new Date(),
          status: TicketStatus.Resolved
        };

        this.supportTicketService.update(updatedTicket).subscribe({
          next: () => {
            this.messageBoxService.show('Chat wurde beendet. Ticket #' + this.currentTicket!.id + ' wurde gespeichert.', 'Erfolg');
            this.isChatActive = false;
            this.messages = [];
            this.currentTicket = null;
          },
          error: (error) => {
            console.error('Error updating ticket:', error);
            this.messageBoxService.show('Fehler beim Beenden des Chats', 'Fehler');
          }
        });
      }
    });
  }

  public markAsResolved(): void {
    if (!this.currentTicket) return;

    this.messageBoxService.confirm(
      'Wurde Ihr Problem gelöst?',
      'Problem gelöst'
    ).then((confirmed: boolean) => {
      if (confirmed && this.currentTicket) {
        const updatedTicket = {
          ...this.currentTicket,
          status: TicketStatus.Resolved,
          endTime: new Date()
        };

        this.supportTicketService.update(updatedTicket).subscribe({
          next: () => {
            const finalMessage: IChatMessage = {
              id: 0,
              content: 'Vielen Dank! Ihr Ticket wurde als gelöst markiert. Wenn Sie weitere Fragen haben, können Sie jederzeit einen neuen Chat starten.',
              isUserMessage: false,
              timestamp: new Date(),
              supportTicketId: this.currentTicket!.id,
              supportTicket: null
            };
            this.messages.push(finalMessage);
            
            setTimeout(() => {
              this.isChatActive = false;
              this.messages = [];
              this.currentTicket = null;
              this.router.navigate(['/dashboard']);
            }, 3000);
          },
          error: (error) => {
            console.error('Error marking as resolved:', error);
            this.messageBoxService.show('Fehler beim Aktualisieren des Tickets', 'Fehler');
          }
        });
      }
    });
  }

  public onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  private scrollToBottom(): void {
    if (this.chatContainer) {
      setTimeout(() => {
        const element = this.chatContainer.nativeElement;
        element.scrollTop = element.scrollHeight;
      }, 50);
    }
  }

  public getMessageTime(timestamp: Date): string {
    const date = new Date(timestamp);
    return date.toLocaleTimeString('de-DE', { hour: '2-digit', minute: '2-digit' });
  }
}
