ALTER PROCEDURE promoteStudents(@Studies NVARCHAR(100), @Semester INTEGER)
AS
BEGIN
	DECLARE @NewSemester INTEGER;
	DECLARE @EnrollmentId INTEGER;
	DECLARE @EnrollmentsCount INTEGER;
	DECLARE @OldEnrollment INTEGER;
	--finding old enrollments number
	SET @OldEnrollment = (SELECT IdEnrollment
					FROM Enrollment 
					JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy
					WHERE Studies.Name = @Studies
					AND Enrollment.Semester = @Semester);
	--finding new enrollments number
	SET @NewSemester = @Semester + 1;
	SET @EnrollmentsCount = (SELECT COUNT(IdEnrollment)
						FROM Enrollment 
						JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy
						WHERE Studies.Name = @Studies
						AND Enrollment.Semester = @NewSemester);
	PRINT @EnrollmentsCount;
	IF @EnrollmentsCount = 0 
		BEGIN
			SET @EnrollmentId = (SELECT MAX(IdEnrollment) FROM Enrollment) + 1;
			DECLARE @IdStudies INTEGER = (SELECT IdStudy FROM Studies WHERE Studies.Name = @Studies);
			INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate)
			VALUES (@EnrollmentId, @NewSemester, @IdStudies, SYSDATETIME());
		END
	ELSE
	BEGIN
		SELECT @EnrollmentId = IdEnrollment  
						FROM Enrollment 
						JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy
						WHERE Studies.Name = @Studies
						AND Enrollment.Semester = @NewSemester;
	END
	--updating
	UPDATE Student 
	SET Student.IdEnrollment = @EnrollmentId
	WHERE Student.IdEnrollment = @OldEnrollment;
END;

