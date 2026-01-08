package cz.route4you.app.ui.screens.selectuser

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cz.route4you.app.domain.User

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SelectUserScreen(
    vm: SelectUserViewModel,
    onUserSelected: () -> Unit
) {
    val state by vm.state.collectAsState()

    LaunchedEffect(Unit) { vm.load() }

    Scaffold(
        topBar = { TopAppBar(title = { Text("Vyber uživatele") }) }
    ) { padding ->
        Box(Modifier.padding(padding).fillMaxSize()) {
            when {
                state.isLoading -> CircularProgressIndicator(Modifier.padding(16.dp))
                state.error != null -> Text("Chyba: ${state.error}", Modifier.padding(16.dp))
                else -> UserList(state.users) { vm.selectUser(it.id, onUserSelected) }
            }
        }
    }
}

@Composable
private fun UserList(users: List<User>, onClick: (User) -> Unit) {
    LazyColumn(
        contentPadding = PaddingValues(12.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        items(users) { u ->
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { onClick(u) }
            ) {
                Column(Modifier.padding(12.dp)) {
                    Text(u.displayName, style = MaterialTheme.typography.titleMedium)
                    Text(u.email, style = MaterialTheme.typography.bodySmall)
                }
            }
        }
    }
}