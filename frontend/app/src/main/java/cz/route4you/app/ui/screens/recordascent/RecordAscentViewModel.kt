package cz.route4you.app.ui.screens.recordascent

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.AscentRepository
import cz.route4you.app.data.local.UserPrefs
import cz.route4you.app.data.remote.RecordAscentDto
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class RecordAscentState(
    val dateText: String = "",          // "2026-01-02" (jednoduché)
    val style: String = "Redpoint",
    val ratingText: String = "",        // "1..5" nebo prázdné
    val notes: String = "",
    val isSubmitting: Boolean = false,
    val error: String? = null,
    val successMessage: String? = null
)

class RecordAscentViewModel(
    private val repo: AscentRepository,
    private val prefs: UserPrefs
) : ViewModel() {

    private val _state = MutableStateFlow(RecordAscentState())
    val state: StateFlow<RecordAscentState> = _state

    fun setDate(text: String) { _state.value = _state.value.copy(dateText = text) }
    fun setStyle(text: String) { _state.value = _state.value.copy(style = text) }
    fun setRating(text: String) { _state.value = _state.value.copy(ratingText = text) }
    fun setNotes(text: String) { _state.value = _state.value.copy(notes = text) }

    fun submit(routeId: String, onDone: () -> Unit) {
        viewModelScope.launch {
            val userId = prefs.userIdFlow.firstOrNull()
            if (userId.isNullOrBlank()) {
                _state.value = _state.value.copy(error = "Není vybraný uživatel.")
                return@launch
            }

            _state.value = _state.value.copy(isSubmitting = true, error = null, successMessage = null)

            // date -> ISO (pro jednoduchost pošleme null nebo "YYYY-MM-DDT00:00:00Z")
            val dateIso = _state.value.dateText.trim().takeIf { it.isNotBlank() }
                ?.let { "${it}T00:00:00Z" }

            val rating = _state.value.ratingText.trim().takeIf { it.isNotBlank() }?.toIntOrNull()

            try {
                repo.record(
                    RecordAscentDto(
                        userId = userId,
                        routeId = routeId,
                        dateClimbed = dateIso,
                        style = _state.value.style,
                        rating = rating,
                        notes = _state.value.notes.trim().takeIf { it.isNotBlank() }
                    )
                )

                _state.value = _state.value.copy(isSubmitting = false, successMessage = "Uloženo ✅")
                onDone()
            } catch (e: Exception) {
                _state.value = _state.value.copy(isSubmitting = false, error = e.message ?: "Submit failed")
            }
        }
    }
}