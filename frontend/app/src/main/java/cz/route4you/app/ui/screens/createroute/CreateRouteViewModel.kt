package cz.route4you.app.ui.screens.createroute

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.RouteRepository
import cz.route4you.app.data.local.UserPrefs
import cz.route4you.app.data.remote.CreateRouteDto
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.launch

data class CreateRouteState(
    val name: String = "",
    val grade: String = "",
    val pitchesText: String = "1",
    val lengthText: String = "",
    val style: String = "Sport",
    val tagsText: String = "",
    val isSaving: Boolean = false,
    val error: String? = null
)

class CreateRouteViewModel(
    private val repo: RouteRepository,
    private val prefs: UserPrefs
) : ViewModel() {

    private val _state = MutableStateFlow(CreateRouteState())
    val state: StateFlow<CreateRouteState> = _state

    fun setName(v: String) { _state.value = _state.value.copy(name = v) }
    fun setGrade(v: String) { _state.value = _state.value.copy(grade = v) }
    fun setPitches(v: String) { _state.value = _state.value.copy(pitchesText = v) }
    fun setLength(v: String) { _state.value = _state.value.copy(lengthText = v) }
    fun setStyle(v: String) { _state.value = _state.value.copy(style = v) }
    fun setTags(v: String) { _state.value = _state.value.copy(tagsText = v) }

    fun save(areaId: String, onSaved: (createdRouteId: String) -> Unit) {
        val name = _state.value.name.trim()
        val grade = _state.value.grade.trim()

        if (name.isBlank() || grade.isBlank()) {
            _state.value = _state.value.copy(error = "Name and grade are required")
            return
        }

        viewModelScope.launch {
            val userId = prefs.userIdFlow.firstOrNull()
            if (userId.isNullOrBlank()) {
                _state.value = _state.value.copy(error = "Není vybraný uživatel.")
                return@launch
            }

            _state.value = _state.value.copy(isSaving = true, error = null)

            val pitches = _state.value.pitchesText.toIntOrNull()?.coerceAtLeast(1) ?: 1
            val length = _state.value.lengthText.trim().takeIf { it.isNotBlank() }?.toIntOrNull()

            val tags = _state.value.tagsText
                .split(",")
                .map { it.trim() }
                .filter { it.isNotBlank() }

            try {
                val id = repo.create(
                    CreateRouteDto(
                        name = name,
                        grade = grade,
                        areaId = areaId,
                        pitches = pitches,
                        lengthMeters = length,
                        style = _state.value.style,
                        tags = tags,
                        createdByUserId = userId
                    )
                )

                _state.value = _state.value.copy(isSaving = false)
                onSaved(id)
            } catch (e: Exception) {
                _state.value = _state.value.copy(isSaving = false, error = e.message ?: "Save failed")
            }
        }
    }
}