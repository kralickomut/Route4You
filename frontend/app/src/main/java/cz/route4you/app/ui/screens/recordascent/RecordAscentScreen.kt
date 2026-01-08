@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.recordascent

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun RecordAscentScreen(
    vm: RecordAscentViewModel,
    routeId: String,
    onBack: () -> Unit,
    onSaved: () -> Unit
) {
    val state by vm.state.collectAsState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Zapsat přelez") },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                    }
                }
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier.padding(padding).padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            // Date (jednoduché textové pole; picker můžeme později)
            OutlinedTextField(
                value = state.dateText,
                onValueChange = vm::setDate,
                label = { Text("Datum (YYYY-MM-DD) – volitelné") },
                modifier = Modifier.fillMaxWidth()
            )

            // Style (simple)
            OutlinedTextField(
                value = state.style,
                onValueChange = vm::setStyle,
                label = { Text("Style (Onsight/Flash/Redpoint/Toprope/Unknown)") },
                modifier = Modifier.fillMaxWidth()
            )

            // Rating
            OutlinedTextField(
                value = state.ratingText,
                onValueChange = vm::setRating,
                label = { Text("Rating 1–5 (volitelné)") },
                modifier = Modifier.fillMaxWidth()
            )

            // Notes
            OutlinedTextField(
                value = state.notes,
                onValueChange = vm::setNotes,
                label = { Text("Poznámky (volitelné)") },
                modifier = Modifier.fillMaxWidth(),
                minLines = 3
            )

            state.error?.let { Text("Chyba: $it") }

            Button(
                onClick = { vm.submit(routeId, onSaved) },
                enabled = !state.isSubmitting,
                modifier = Modifier.fillMaxWidth()
            ) {
                Text(if (state.isSubmitting) "Ukládám..." else "Uložit")
            }
        }
    }
}