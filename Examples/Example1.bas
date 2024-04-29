# Define a function
DEF ADD(A, B) DO
    # You can define a function inside another function
    DEF SUB(C, D) DO
        RETURN C - D
    END
    RETURN SUB(A, B)
END

# Call the function
SUM = ADD(1, 2)
PRINT(SUM)

# This is a comment
A = 1
B = 3.14
C = TRUE
D = FALSE

# This is a string
E = "Moon"

# String interpolation
PRINT("Goodnight {E}")

# Variables only! No complex expressions. This is invalid:
# PRINT("Goodnight {A + B}")

IF A == 1 DO
  PRINT("A is 1")
END

# For loops work over ranges of integers (inclusive)
FOR I = 1 TO 10 DO
  PRINT(I)
END

# While loops
F = 10
WHILE F > 0 DO
  PRINT(F)
  F = F - 1
END

# Break statement
G = 0
WHILE TRUE DO
  G = G + 1
  IF G == 10 DO
    BREAK
  END
END
PRINT("G is {G}")

# Types are dynamic
# H is a number
H = 3.14
# Now H is a string
H = "Pi"

DEF GREET(NAME) DO
  PRINT("Hello {NAME}!")
END

DEF SAY_SOMETHING(GREETER) DO
  GREETER("World")
END

# Functions can be passed around as arguments
SAY_SOMETHING(GREET)
