# TCP Roadmap

This document outlines the future development plans for TCP (Train Control Platform).

## Current Status (TCP-1.0.1)

- ✅ UI Foundation
- ✅ Settings & Persistence
- ✅ Info Panel & Documentation
- ✅ Home Map Canvas (Empty)

## Planned Features

### TCP-1.0.2 — Map Image Loading

**Goal**: Load map images into the canvas

- Image file selection (OpenFileDialog)
- Image loading and display in canvas
- Image scaling and positioning
- Basic image manipulation (zoom, pan)

### TCP-1.0.3 — Boxes (Placement)

**Goal**: Place boxes on the map

- Box placement tool
- Box selection and manipulation
- Box properties panel
- Box persistence (save/load)

### TCP-1.0.4 — Lock System

**Goal**: Lock/unlock boxes

- Lock/unlock functionality
- Visual feedback for locked boxes
- Lock state persistence

### TCP-1.0.5 — Save/Load

**Goal**: Save and load map projects

- Project file format (JSON)
- Save project dialog
- Load project dialog
- Project file validation

### TCP-1.0.6 — Status Integration

**Goal**: Integrate with Electronics status

- Box status display
- Real-time status updates
- Status visualization on map

## Future Milestones

### TCP-2.0.0 — Simulation Core

- Simulation logic and physics engine
- Train movement simulation
- Real-time simulation controls

### TCP-3.0.0 — Hardware Bridge

- Real hardware integration (Arduino, RFID, Servo controllers)
- Live train tracking and position monitoring
- Servo and switch control interface
- Network communication protocols

### TCP-4.0.0 — Advanced Features

- Multi-board coordination
- Advanced simulation with physics engine
- Network communication protocols
- Advanced visualization

## Development Principles

- **Incremental development**: Small, focused releases
- **Stable foundation**: Each release builds on stable base
- **No breaking changes**: Maintain backward compatibility
- **User feedback**: Incorporate user feedback in each release

## Notes

- All features follow existing MVVM pattern
- All features use theme tokens
- All features have error guardrails
- All features are versioned and documented

