package cz.route4you.app.ui.screens.createarea

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.AreaRepository
import cz.route4you.app.data.remote.CreateAreaDto
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class CreateAreaState(
    val name: String = "",
    val type: String = "Country", // default
    val isSaving: Boolean = false,
    val error: String? = null
)

class CreateAreaViewModel(private val repo: AreaRepository) : ViewModel() {
    private val _state = MutableStateFlow(CreateAreaState())
    val state: StateFlow<CreateAreaState> = _state

    fun setName(v: String) { _state.value = _state.value.copy(name = v) }
    fun setType(v: String) { _state.value = _state.value.copy(type = v) }

    fun save(parentId: String?, onSaved: () -> Unit) {
        val name = _state.value.name.trim()
        if (name.isBlank()) {
            _state.value = _state.value.copy(error = "Name is required")
            return
        }

        viewModelScope.launch {
            _state.value = _state.value.copy(isSaving = true, error = null)
            try {
                repo.create(
                    CreateAreaDto(
                        name = name,
                        type = _state.value.type,
                        parentId = parentId
                    )
                )
                _state.value = _state.value.copy(isSaving = false)
                onSaved()
            } catch (e: Exception) {
                _state.value = _state.value.copy(isSaving = false, error = e.message ?: "Save failed")
            }
        }
    }
}