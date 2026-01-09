# 📘 Generische Angular-Frontend & Workflow Anleitung  

# ## Teil A: Projektstruktur & Setup

### **1. Neues Angular-Projekt erstellen**

```bash
ng new <project-name>
```

2. Routing aktivieren
Routing beim Erstellen aktivieren oder später manuell ergänzen.

3. UI-Framework integrieren
Wahlweise:

Angular Material

Bootstrap

4. Empfohlene Projektstruktur
css
Copy code
src/app/
  core/            → Services, Interceptors, Guards
  shared/          → Models, UI-Komponenten, Pipes
  pages/           → Feature-Seiten (Listenansichten)
    entities/
  components/      → Edit-/Form-Komponenten

## Teil B: Entity-Service (CRUD)

Für jede Entität wird ein eigener Service erstellt.

Beispielmethoden in der ts file:

- getAll(): Observable<Entity[]>
- getById(id: string): Observable<Entity>
- create(entity: Entity): Observable<Entity>
- update(entity: Entity): Observable<Entity>
- delete(id: string): Observable<void>

API-Grundkonvention:

- GET    /api/<entity>
- GET    /api/<entity>/:id
- POST   /api/<entity>
- PUT    /api/<entity>/:id
- DELETE /api/<entity>/:id
  
Empfehlung
Interfaces definieren für Typisierung

Zentrales API-URL in environment.ts

## Angular Komponenten

Die Generierung der Komponenten erfolgt für die Listen die sich im Ordner 'src/app/pages/entities/' befinden. Die dazugehörigen Edit Komponenten befinden sich im Ordner 'src/app/components/entities'.

### List Component Template

```html
<div class="container mt-4">
  <!-- Header -->
  <div class="d-flex justify-content-between align-items-center mb-4 p-3 bg-secondary text-white shadow-sm rounded">
    <h3 class="mb-0 flex-grow-1">
      {{ 'ENTITYNAME_LIST.TITLE_PLURAL' | translate }}
    </h3>
    <button
      class="btn btn-outline-light"
      (click)="navigateBack()"
      title="{{ 'ENTITYNAME_LIST.BACK' | translate }}"
      data-bs-toggle="tooltip"
      data-bs-placement="top">
      <i class="bi bi-arrow-left-circle"></i>
    </button>
  </div>

  <!-- Search and Add Section -->
  <div class="card p-3 mb-4 shadow-sm">
    <div class="d-flex flex-column flex-md-row align-items-md-center gap-2">
      <div class="input-group">
        <span class="input-group-text">
          <i class="bi bi-search"></i>
        </span>
        <input
          type="text"
          class="form-control"
          placeholder="{{ 'ENTITYNAME_LIST.SEARCH_PLACEHOLDER' | translate }}"
          [(ngModel)]="searchTerm"
          (input)="onSearchChange()" />
      </div>

      <button
        class="btn btn-primary ms-md-2"
        (click)="openCreateModal()"
        [title]="'ENTITYNAME_LIST.ADD' | translate">
        <i class="bi bi-plus-lg"></i>
        {{ 'ENTITYNAME_LIST.ADD' | translate }}
      </button>
    </div>
  </div>

  <!-- Entity List -->
  <ul class="list-group">
    <li
      *ngFor="let entity of filteredEntities"
      class="list-group-item list-group-item-action flex-column flex-md-row d-flex align-items-start align-items-md-center justify-content-between mb-2 shadow-sm">

      <!-- Entity Information -->
      <div class="flex-grow-1">
        <div class="fw-bold mb-1">
          <i class="bi bi-collection"></i>
          <span class="ms-1">
            {{ entity.name || ('ENTITYNAME_LIST.NO_NAME' | translate) }}
          </span>
        </div>
        <div class="small text-muted mb-2" *ngIf="entity.id">
          <i class="bi bi-card-text"></i>
          <span class="ms-1">
            ID: {{ entity.id }}
          </span>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="d-flex gap-2">
        <button
          class="btn btn-sm btn-outline-success"
          (click)="openUpdateModal(entity)"
          [title]="'ENTITYNAME_LIST.UPDATE' | translate">
          <i class="bi bi-pencil-square"></i>
        </button>

        <button
          class="btn btn-sm btn-outline-danger"
          (click)="deleteEntity(entity)"
          [title]="'ENTITYNAME_LIST.DELETE' | translate">
          <i class="bi bi-trash"></i>
        </button>
      </div>
    </li>

    <!-- No Results -->
    <li *ngIf="filteredEntities.length === 0" class="list-group-item text-muted text-center">
      {{ 'ENTITYNAME_LIST.NO_RESULTS' | translate }}
    </li>
  </ul>
</div>


```

### Bearbeitungsansicht (Edit-Formular)

* Für die Ansicht ist eine **Bootstrap-Card-Ansicht** zu verwenden.
* Die Komponenten sind bereits erstellt und befinden sich im Ordner `src/app/components/entities`.
* Alle Komponenten sind `standalone` Komponenten.
* **Dateiname:** `entity-name-edit.component.html`
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien `de.json` und `en.json` um die hinzugefügten Labels.
* Beispielstruktur:

```html

<div *ngIf="dataItem" class="card mt-4 shadow-sm">
  <!-- Header -->
  <div class="card-header d-flex justify-content-between align-items-center">
    <h3 class="mb-0">
      {{
        (editMode
          ? 'ENTITYNAME_EDIT.TITLE_EDIT'
          : 'ENTITYNAME_EDIT.TITLE_CREATE') | translate
      }}
    </h3>
    <button
      type="button"
      class="btn btn-sm btn-danger"
      aria-label="Close"
      (click)="dismiss()"
    >
      <span aria-hidden="true">&times;</span>
    </button>
  </div>

  <!-- Body -->
  <div class="card-body">
    <form (ngSubmit)="submitForm()" #editForm="ngForm">
      <!-- Name -->
      <div class="mb-3">
        <label class="form-label">{{
          'ENTITYNAME_EDIT.LABEL_NAME' | translate
        }}</label>
        <input
          class="form-control"
          [(ngModel)]="dataItem.name"
          name="name"
          required
        />
      </div>

      <!-- Description -->
      <div class="mb-3">
        <label class="form-label">{{
          'ENTITYNAME_EDIT.LABEL_DESCRIPTION' | translate
        }}</label>
        <textarea
          class="form-control"
          [(ngModel)]="dataItem.description"
          name="description"
          rows="3"
        ></textarea>
      </div>

      <!-- Optional: Dropdown für abhängige Entitäten -->
      <div class="mb-3" *ngIf="relatedEntities.length > 0">
        <label class="form-label">{{
          'ENTITYNAME_EDIT.LABEL_RELATED' | translate
        }}</label>
        <select
          class="form-select"
          [(ngModel)]="dataItem.relatedEntityId"
          name="relatedEntityId"
        >
          <option [ngValue]="null">
            {{ 'ENTITYNAME_EDIT.SELECT_RELATED' | translate }}
          </option>
          <option
            *ngFor="let related of relatedEntities"
            [ngValue]="related.id"
          >
            {{ related.name }}
          </option>
        </select>
      </div>

      <!-- Optional: Datum -->
      <div class="mb-3" *ngIf="dataItem.date !== undefined">
        <label class="form-label">{{
          'ENTITYNAME_EDIT.LABEL_DATE' | translate
        }}</label>
        <input
          type="date"
          class="form-control"
          [(ngModel)]="dateString"
          name="date"
        />
      </div>

      <!-- Buttons -->
      <div class="d-flex justify-content-end gap-2 mt-4">
        <button
          type="submit"
          class="btn btn-success"
          [disabled]="editForm.invalid || saveData"
        >
          <i class="bi bi-check-circle me-1"></i>
          {{ 'COMMON.SAVE' | translate }}
        </button>
        <button
          type="button"
          class="btn btn-secondary"
          (click)="cancelForm()"
          [disabled]="saveData"
        >
          <i class="bi bi-x-circle me-1"></i>
          {{ 'COMMON.CANCEL' | translate }}
        </button>
      </div>
    </form>
  </div>
</div>


```

### Master-Details

In manchen Fällen ist eine Master/Details Ansicht sehr hilfreich. Diese Anzeige besteht aus einer Master-Ansicht. Diese kann nicht bearbeitet werden. Die Details zu diesem Master werden unter der Master-Ansicht als 'List-group' angezeigt. Die Generierung soll nur nach Aufforderung des Benutzers erfolgen. Nachfolgend ist die Struktur skizziert:

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { IdType } from '@app/models/i-key-model';
import { IMasterEntity } from '@app-models/entities/app/i-master-entity';
import { IDetailEntity } from '@app-models/entities/app/i-detail-entity';

import { MasterService } from '@app-services/http/entities/app/master-service';
import { DetailService } from '@app-services/http/entities/app/detail-service';
import { MasterXDetailService } from '@app-services/http/entities/app/master-x-detail-service';
import { DetailEditComponent } from '@app/components/entities/app/detail-edit.component';

@Component({
  selector: 'app-master-detail-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './master-detail-list.component.html',
  styleUrl: './master-detail-list.component.css',
})
export class MasterDetailListComponent implements OnInit {
  public masterEntity: IMasterEntity = {} as IMasterEntity;
  public masterDetails: IDetailEntity[] = [];
  public availableDetails: IDetailEntity[] = [];
  public selectedDetailId: IdType = 0;

  private masterId: IdType = 0;

  constructor(
    private masterService: MasterService,
    private detailService: DetailService,
    private masterDetailService: MasterXDetailService,
    private modal: NgbModal
  ) {}

  ngOnInit(): void {
    this.masterId = Number(window.location.pathname.split('/').pop() || '0');
    if (isNaN(this.masterId) || this.masterId <= 0) {
      console.error('Invalid masterId in URL:', this.masterId);
      return;
    }
    this.loadMaster(this.masterId);
    this.loadMasterDetails(this.masterId);
    this.loadAvailableDetails(this.masterId);
  }

  private loadMaster(masterId: IdType): void {
    this.masterService.getById(masterId).subscribe((entity) => {
      this.masterEntity = entity;
    });
  }

  private loadMasterDetails(masterId: IdType): void {
    this.masterDetailService.detailsByMasterId(masterId).subscribe((details) => {
      this.masterDetails = details || [];
    });
  }

  private loadAvailableDetails(masterId: IdType): void {
    this.masterDetailService.availableDetails(masterId).subscribe((details) => {
      this.availableDetails = details || [];
    });
  }

  public addDetailToMaster(): void {
    if (this.selectedDetailId === 0) return;
    this.masterDetailService
      .addDetailToMaster(this.masterId, this.selectedDetailId)
      .subscribe({
        next: () => {
          this.loadMasterDetails(this.masterId);
          this.loadAvailableDetails(this.masterId);
          this.selectedDetailId = 0;
        },
        error: (err) => console.error('Error adding detail:', err),
      });
  }

  public openCreateDetailModal(): void {
    this.detailService.getTemplate().subscribe({
      next: (template) => {
        const ref = this.modal.open(DetailEditComponent, {
          size: 'lg',
          centered: true,
        });
        ref.componentInstance.dataItem = template;

        if (ref.componentInstance.save) {
          ref.componentInstance.save.subscribe((item: IDetailEntity) => {
            ref.componentInstance.saveData = true;
            this.detailService.create(item).subscribe({
              next: () => {
                this.loadAvailableDetails(this.masterId);
                this.loadMasterDetails(this.masterId);
                ref.close();
                ref.componentInstance.saveData = false;
              },
              error: (err) => {
                console.error('Error creating detail:', err);
                ref.componentInstance.saveData = false;
              },
            });
          });
        }
      },
    });
  }

  public removeDetailFromMaster(item: IDetailEntity): void {
    this.masterDetailService
      .removeDetailFromMaster(item.id, this.masterId)
      .subscribe({
        next: () => {
          this.loadMasterDetails(this.masterId);
          this.loadAvailableDetails(this.masterId);
        },
        error: (err) => console.error('Error removing detail:', err),
      });
  }
}

```

```html
<div class="container mt-4">
  <!-- Header -->
  <div
    class="d-flex justify-content-between align-items-center mb-4 p-3 bg-secondary text-white shadow-sm rounded"
  >
    <h3 class="mb-0 flex-grow-1">
      {{ 'MASTERDETAIL_LIST.TITLE' | translate }}
      {{ masterEntity?.displayName || '' }}
    </h3>

    <a
      routerLink="/{{ masterEntityPlural }}"
      class="btn btn-outline-light"
      title="{{ 'MASTERDETAIL_LIST.BACK' | translate }}"
      data-bs-toggle="tooltip"
      data-bs-placement="top"
    >
      <i class="bi bi-arrow-left-circle"></i>
    </a>
  </div>

  <!-- Add Detail Section -->
  <div class="card p-3 mb-4 shadow-sm">
    <div class="d-flex flex-column flex-md-row align-items-md-center gap-2">
      <select
        id="detailSelect"
        name="detailSelect"
        class="form-select"
        [(ngModel)]="selectedDetailId"
      >
        <option [ngValue]="0">
          {{ 'MASTERDETAIL_LIST.SELECT_DETAIL' | translate }}
        </option>
        <option *ngFor="let detail of availableDetails" [ngValue]="detail.id">
          {{ detail.displayName || ('MASTERDETAIL_LIST.NO_NAME' | translate) }}
        </option>
      </select>

      <button
        class="btn btn-primary"
        (click)="addDetailToMaster()"
        [disabled]="selectedDetailId === 0"
        [title]="'MASTERDETAIL_LIST.ADD_DETAIL' | translate"
      >
        <i class="bi bi-plus-lg"></i>
        {{ 'MASTERDETAIL_LIST.ADD_DETAIL' | translate }}
      </button>

      <button
        class="btn btn-outline-success"
        (click)="openCreateDetailModal()"
        [title]="'MASTERDETAIL_LIST.CREATE_DETAIL' | translate"
      >
        <i class="bi bi-plus-circle"></i>
        {{ 'MASTERDETAIL_LIST.CREATE_DETAIL' | translate }}
      </button>
    </div>

    <div class="invalid-feedback d-block mt-1" *ngIf="selectedDetailId === 0">
      {{ 'MASTERDETAIL_LIST.SELECT_DETAIL_REQUIRED' | translate }}
    </div>
  </div>

  <!-- Master Detail List -->
  <ul class="list-group">
    <li
      *ngFor="let item of masterDetails"
      class="list-group-item list-group-item-action flex-column flex-md-row d-flex align-items-start align-items-md-center justify-content-between mb-2 shadow-sm"
    >
      <div class="flex-grow-1">
        <div class="fw-bold mb-1">
          <i class="bi bi-card-checklist"></i>
          <span class="ms-1">
            {{ item.displayName || ('MASTERDETAIL_LIST.NO_NAME' | translate) }}
          </span>
        </div>
        <div class="small text-muted mb-2" *ngIf="item.id">
          <i class="bi bi-card-text"></i>
          <span class="ms-1">
            {{ 'MASTERDETAIL_LIST.DETAIL_ID' | translate }}: {{ item.id }}
          </span>
        </div>
      </div>

      <button
        class="btn btn-sm btn-outline-danger"
        (click)="removeDetailFromMaster(item)"
        [title]="'MASTERDETAIL_LIST.REMOVE' | translate"
      >
        <i class="bi bi-trash"></i>
      </button>
    </li>

    <li
      *ngIf="masterDetails.length === 0"
      class="list-group-item text-center text-muted"
    >
      {{ 'MASTERDETAIL_LIST.NO_DETAILS' | translate }}
    </li>
  </ul>
</div>


```


## Teil C: Listenansichten (ListComponents)

Ort
swift
Copy code
src/app/pages/entities/
Features
Tabellen- oder Card-Darstellung

Filtermöglichkeiten:

Dropdown für Kategorien

Dropdown für Fremdschlüssel

Suchfeld für Text

Aktionen:

Bearbeiten

Löschen

Optional:

Pagination

Sorting

Responsive Design
Bootstrap Cards oder Material Tables verwenden

## Teil D: Edit-/Form-Komponenten

Ort
swift
Copy code
src/app/components/entities/
Technologie
Reactive Forms

Validierungsbeispiele
Pflichtfelder

Minimale Zeichenlänge

Positive numerische Werte

Datum in der Zukunft

Pattern-Validierung

Automatisch durch KI ableitbar
Formularfelder basierend auf Properties

Dropdowns für Relationen

Selects für Enums

Datepicker für Datumseingaben

## Teil E: Dashboard

DashboardComponent
Empfohlene Inhalte:

Gesamtanzahl von Einträgen

Markierte Einträge (z. B. kritisch niedriger Bestand)

Elemente mit baldigem Ablauf-/Erneuerungsbedarf

Navigation zu allen Entitätsmodulen

Verwendung:

Bootstrap Cards / Material Cards

## Teil G: Routing

1. List-Komponenten registrieren
In app-routing.module.ts:

ts
Copy code
{ path: '<entity>', component: EntityListComponent }
2. Edit-Komponenten
ts
Copy code
{ path: '<entity>/create', component: EntityEditComponent }
{ path: '<entity>/edit/:id', component: EntityEditComponent }
3. Dashboard erweitern
Navigationslinks für alle Entitäten hinzufügen

## Teil H: Internationalisierung (i18n)

Dateien
bash
Copy code
src/assets/i18n/de.json
src/assets/i18n/en.json
Schlüsselkonvention
Copy code
ENTITY_LIST.*
ENTITY_EDIT.*
Beispiel
json
Copy code
{
  "ENTITY_LIST": {
    "TITLE_PLURAL": "Entities",
    "BACK": "Back",
    "SEARCH_PLACEHOLDER": "Search...",
    "ADD": "Add",
    "NO_RESULTS": "No results found"
  },
  "ENTITY_EDIT": {
    "TITLE_EDIT": "Edit Entity",
    "TITLE_CREATE": "Create Entity",
    "LABEL_NAME": "Name",
    "LABEL_DESCRIPTION": "Description",
    "LABEL_RELATED": "Related Entity"
  }
}

## Teil I: Styling

Global
src/styles.css
→ Grundlegende Layoutregeln, Farben, Typografie

Komponenten-spezifisch
<component>.css
→ Detailstyling je Komponente

## Teil J: Checkliste für vollständige Implementierung

Frontend
 List Component zeigt Daten

 Create-Form/Modal speichert korrekt

 Edit-Komponente lädt Daten & speichert

 Delete mit Bestätigungsdialog

 Suche & Filter arbeiten korrekt

 Dropdowns für Relationen funktionieren

 Validierung greift im Frontend

 Backend-Validierung wird abgefangen

Internationalisierung
 Alle Labels in DE/EN vorhanden

 Sprachwechsel funktioniert

## Zusatz: KI-Optimierungen

1. Klare Namenskonventionen
<entity>.model.ts

<entity>.service.ts

EntityListComponent

EntityEditComponent

2. Einheitliche API-Struktur
Erhöht die Chance, dass KI automatisch richtigen Code generiert.

3. Kontextblock für KI
Alle folgenden Schritte beziehen sich auf ein generisches Angular CRUD Projekt.
Leite Formulare, Services und Routing automatisch aus den Entitätsdaten ab.

4. Empfehlung
Validierungsregeln in JSON definieren

Einheitliche Dateistruktur für Entities

Eingabefelder alphabetisch sortieren

